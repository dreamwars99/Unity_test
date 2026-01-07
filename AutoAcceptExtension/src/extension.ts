import * as vscode from 'vscode';

// 타이머 변수를 선언합니다. 사용자가 타이핑을 멈췄는지 확인하기 위해 사용됩니다.
let acceptTimer: NodeJS.Timeout | undefined;

// 자동 수락 기능의 활성화 상태를 저장하는 변수입니다.
let isAutoAcceptEnabled = false;

// 상태 표시줄 아이템 (화면 하단에 ON/OFF 상태를 보여줍니다)
let statusBarItem: vscode.StatusBarItem;

export function activate(context: vscode.ExtensionContext) {
    console.log('Auto Accept Extension is now active!');

    // 1. 상태 표시줄 아이템 생성 및 초기화
    statusBarItem = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Right, 100);
    statusBarItem.command = 'autoAccept.toggle';
    context.subscriptions.push(statusBarItem);
    updateStatusBar();

    // 2. 명령어 등록: 기능을 켜고 끄는 명령어입니다.
    let toggleCommand = vscode.commands.registerCommand('autoAccept.toggle', () => {
        isAutoAcceptEnabled = !isAutoAcceptEnabled;
        updateStatusBar();
        
        if (isAutoAcceptEnabled) {
            vscode.window.showInformationMessage('자동 수락(Auto Accept) 기능이 켜졌습니다. 🚀');
        } else {
            vscode.window.showInformationMessage('자동 수락(Auto Accept) 기능이 꺼졌습니다. ⏸️');
        }
    });

    context.subscriptions.push(toggleCommand);

    // 3. 텍스트 변경 감지 이벤트 리스너 등록
    // 사용자가 타이핑을 하거나 커서를 움직일 때마다 이 이벤트가 발생할 수 있습니다.
    // 여기서는 '타이핑'을 감지하기 위해 onDidChangeTextDocument를 사용합니다.
    vscode.workspace.onDidChangeTextDocument(event => {
        // 기능이 꺼져있거나, 활성화된 에디터가 없으면 무시합니다.
        if (!isAutoAcceptEnabled || !vscode.window.activeTextEditor) {
            return;
        }

        // 사용자가 타이핑 중이라면 기존 타이머를 취소합니다 (아직 수락하지 않음).
        if (acceptTimer) {
            clearTimeout(acceptTimer);
        }

        // 설정된 지연 시간(기본값 600ms)을 가져옵니다.
        const config = vscode.workspace.getConfiguration('autoAccept');
        const delay = config.get<number>('delay') || 600;

        // 새로운 타이머를 설정합니다.
        // 지정된 시간(delay) 동안 추가 입력이 없으면 내부의 코드가 실행됩니다.
        acceptTimer = setTimeout(() => {
            tryAcceptSuggestion();
        }, delay);
    }, null, context.subscriptions);
}

// 제안된 코드를 수락하는 함수입니다.
async function tryAcceptSuggestion() {
    const editor = vscode.window.activeTextEditor;
    if (!editor) {
        return;
    }

    try {
        // VS Code의 내장 명령어인 'inlineSuggest.commit'을 실행합니다.
        // 이 명령어는 현재 회색 텍스트(Ghost Text)로 보이는 제안이 있다면 그것을 수락합니다.
        // 제안이 없다면 아무 일도 일어나지 않습니다.
        await vscode.commands.executeCommand('editor.action.inlineSuggest.commit');
        
        // (선택 사항) 수락 후 로그를 남기고 싶다면 아래 주석을 해제하세요.
        // console.log('제안을 자동으로 수락했습니다.');
    } catch (error) {
        console.error('자동 수락 중 오류 발생:', error);
    }
}

// 상태 표시줄의 텍스트와 색상을 업데이트하는 함수입니다.
function updateStatusBar() {
    if (isAutoAcceptEnabled) {
        statusBarItem.text = '$(check) Auto Accept: ON';
        statusBarItem.backgroundColor = new vscode.ThemeColor('statusBarItem.warningBackground'); // 눈에 띄게 배경색 변경
        statusBarItem.tooltip = '클릭하여 자동 수락 기능을 끕니다.';
    } else {
        statusBarItem.text = '$(circle-slash) Auto Accept: OFF';
        statusBarItem.backgroundColor = undefined;
        statusBarItem.tooltip = '클릭하여 자동 수락 기능을 켭니다.';
    }
    statusBarItem.show();
}

export function deactivate() {
    // 확장 프로그램이 종료될 때 타이머를 정리합니다.
    if (acceptTimer) {
        clearTimeout(acceptTimer);
    }
}
