using Godot;
using System.Collections.Generic;

/// <summary>
/// 대화 출력, 자동 진행, 선택지 처리를 담당합니다.
/// </summary>
public partial class DialogueSceneController : Control
{
	private Label _expressionLabel = null!;
	private Label _nameLabel = null!;
	private Label _bodyLabel = null!;
	private Label _progressLabel = null!;

	private VBoxContainer _choiceBox = null!;
	private Button _choiceButton1 = null!;
	private Button _choiceButton2 = null!;
	private Button _choiceButton3 = null!;

	private GameManager _gameManager = null!;

	private readonly List<DialogueLine> _dialogueLines = new();
	private readonly List<DialogueChoice> _currentChoices = new();

	private int _currentLineIndex = 0;
	private bool _dialogueFinished = false;
	private bool _waitingForChoice = false;
	private bool _inputLocked = false;

	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");

		FindUiNodes();
		ConnectChoiceButtons();
		CreateSampleDialogue();

		HideChoices();

		_currentLineIndex = 0;
		ShowCurrentLine();

	}

	private void FindUiNodes()
	{
		_expressionLabel = GetNode<Label>(
            "DialogueMargin/DialogueBox/ExpressionLabel"
		);

		_nameLabel = GetNode<Label>(
            "DialogueMargin/DialogueBox/NameLabel"
		);

		_bodyLabel = GetNode<Label>(
            "DialogueMargin/DialogueBox/BodyLabel"
		);

		_progressLabel = GetNode<Label>(
            "DialogueMargin/DialogueBox/ProgressLabel"
		);

		_choiceBox = GetNode<VBoxContainer>(
            "DialogueMargin/DialogueBox/ChoiceBox"
		);

		_choiceButton1 = GetNode<Button>(
            "DialogueMargin/DialogueBox/ChoiceBox/ChoiceButton1"
		);

		_choiceButton2 = GetNode<Button>(
            "DialogueMargin/DialogueBox/ChoiceBox/ChoiceButton2"
		);

		_choiceButton3 = GetNode<Button>(
            "DialogueMargin/DialogueBox/ChoiceBox/ChoiceButton3"
		);
	}

	private void ConnectChoiceButtons()
	{
		_choiceButton1.Pressed += () => SelectChoice(0);
		_choiceButton2.Pressed += () => SelectChoice(1);
		_choiceButton3.Pressed += () => SelectChoice(2);
	}

	private void CreateSampleDialogue()
	{
		_dialogueLines.Clear();

		_dialogueLines.Add(new DialogueLine(
			"재윤",
			"오늘은 자소서에 쓸 내용을 정리하겠습니다.",
            "Serious"
		));

		_dialogueLines.Add(new DialogueLine(
			"벨",
			"마계를 정복한 위업을 첫 줄에 적어라.",
            "Proud"
		));

		_dialogueLines.Add(new DialogueLine(
			"재윤",
			"그 전에 묻겠습니다. 제 말을 조금이라도 믿습니까?",
            "Neutral"
		));

		_dialogueLines.Add(new DialogueLine(
			"재윤",
			"그럼 다음 내용을 계속 정리하죠.",
            "Serious"
		));

		_dialogueLines.Add(new DialogueLine(
			"벨",
			"좋다. 인간의 문서 작성법을 정복해 주마.",
            "Proud"
		));
	}

	private void ShowCurrentLine()
	{
		if (_currentLineIndex < 0 ||
			_currentLineIndex >= _dialogueLines.Count)
		{
			FinishDialogue();
			return;
		}

		DialogueLine currentLine = _dialogueLines[_currentLineIndex];

		_nameLabel.Text = currentLine.SpeakerName;
		_bodyLabel.Text = currentLine.BodyText;
		_expressionLabel.Text = $"표정: {currentLine.Expression}";
		_progressLabel.Text =
			$"{_currentLineIndex + 1} / {_dialogueLines.Count}";

		GD.Print(
			$"[{currentLine.Expression}] " +
			$"{currentLine.SpeakerName}: " +
			$"{currentLine.BodyText}"
		);

		// 세 번째 대사에서 선택지를 표시합니다.
		if (_currentLineIndex == 2)
		{
			ShowTrustChoices();
		}
	}

	private void ShowTrustChoices()
	{
		_waitingForChoice = true;
		_currentChoices.Clear();

		_currentChoices.Add(new DialogueChoice(
			"믿는다",
			1,
			"TRUST_JAEYOON",
			new DialogueLine(
				"재윤",
				"그 말은 기억해 두겠습니다.",
                "Surprised"
			)
		));

		_currentChoices.Add(new DialogueChoice(
			"아직 모르겠다",
			0,
			"UNSURE_JAEYOON",
			new DialogueLine(
				"재윤",
				"당장 대답하기 어렵다는 건 이해합니다.",
                "Neutral"
			)
		));

		_currentChoices.Add(new DialogueChoice(
			"믿지 않는다",
			-1,
			"DISTRUST_JAEYOON",
			new DialogueLine(
				"재윤",
				"솔직한 대답이군요. 예상은 했습니다.",
                "Tired"
			)
		));

		_choiceButton1.Text = _currentChoices[0].Text;
		_choiceButton2.Text = _currentChoices[1].Text;
		_choiceButton3.Text = _currentChoices[2].Text;

		_choiceBox.Visible = true;

		GD.Print("선택지를 기다리는 중입니다.");
	}

	private void SelectChoice(int choiceIndex)
	{
		if (!_waitingForChoice)
		{
			return;
		}

		if (choiceIndex < 0 || choiceIndex >= _currentChoices.Count)
		{
			GD.PushError($"잘못된 선택지 번호: {choiceIndex}");
			return;
		}

		DialogueChoice selectedChoice = _currentChoices[choiceIndex];

		_waitingForChoice = false;
		HideChoices();

		_gameManager.ChangeJaeyoonAffection(
			selectedChoice.AffectionChange
		);

		_gameManager.AddFlag(selectedChoice.FlagId);

		ShowResultLine(selectedChoice.ResultLine);
		LockInputBriefly();

		GD.Print($"선택 결과: {selectedChoice.Text}");
		GD.Print(
			$"현재 재윤 호감도: " +
			$"{_gameManager.CurrentState.JaeyoonAffection}"
		);
	}

	private void ShowResultLine(DialogueLine resultLine)
	{
		_nameLabel.Text = resultLine.SpeakerName;
		_bodyLabel.Text = resultLine.BodyText;
		_expressionLabel.Text = $"표정: {resultLine.Expression}";
		_progressLabel.Text = "선택 결과";

		GD.Print(
			$"[{resultLine.Expression}] " +
			$"{resultLine.SpeakerName}: " +
			$"{resultLine.BodyText}"
		);
	}

	private void AdvanceDialogue()
	{
		_currentLineIndex++;

		if (_currentLineIndex >= _dialogueLines.Count)
		{
			FinishDialogue();
			return;
		}

		ShowCurrentLine();
	}

	private void HideChoices()
	{
		_choiceBox.Visible = false;
	}

	private void FinishDialogue()
	{
		if (_dialogueFinished)
		{
			return;
		}

		_dialogueFinished = true;
		HideChoices();

		_nameLabel.Text = "";
		_bodyLabel.Text =
			"샘플 선택지 대화가 종료되었습니다.";

		_expressionLabel.Text = "표정: 없음";

		_progressLabel.Text =
			$"재윤 호감도: " +
			$"{_gameManager.CurrentState.JaeyoonAffection}";

		GD.Print("선택지 테스트 완료");

		PrintFlags();
	}

	private void PrintFlags()
	{
		GD.Print("===== 현재 이벤트 플래그 =====");

		foreach (string flag in
				 _gameManager.CurrentState.EventFlags)
		{
			GD.Print(flag);
		}

		GD.Print("============================");
	}
	
	public override void _Input(InputEvent @event)
	{
		if (_dialogueFinished ||
			_waitingForChoice ||
			_inputLocked)
		{
			return;
		}

		if (@event is InputEventMouseButton mouseButton &&
			mouseButton.ButtonIndex == MouseButton.Left &&
			mouseButton.Pressed)
		{
			AdvanceDialogue();
			GetViewport().SetInputAsHandled();
		}
	}
	
	private async void LockInputBriefly()
	{
		_inputLocked = true;

		await ToSignal(
			GetTree().CreateTimer(0.15),
			SceneTreeTimer.SignalName.Timeout
		);

		_inputLocked = false;
	}
}
