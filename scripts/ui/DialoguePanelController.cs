using Godot;
using System.Collections.Generic;

/// <summary>
/// Reusable dialogue UI panel. Hosts provide dialogue lines and choices.
/// </summary>
public partial class DialoguePanelController : Control
{
	[Signal]
	public delegate void DialogueFinishedEventHandler();

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
	private readonly Dictionary<int, List<DialogueChoice>> _choicesByLine = new();
	private readonly List<DialogueChoice> _currentChoices = new();

	private int _currentLineIndex = 0;
	private bool _isDialogueRunning = false;
	private bool _dialogueFinished = false;
	private bool _waitingForChoice = false;
	private bool _inputLocked = false;

	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");

		FindUiNodes();
		ConnectChoiceButtons();
		HideChoices();
	}

	public void StartDialogue(
		List<DialogueLine> lines,
		Dictionary<int, List<DialogueChoice>> choicesByLine
	)
	{
		_dialogueLines.Clear();
		_choicesByLine.Clear();
		_currentChoices.Clear();

		if (lines is not null)
		{
			_dialogueLines.AddRange(lines);
		}

		if (choicesByLine is not null)
		{
			foreach (KeyValuePair<int, List<DialogueChoice>> entry in choicesByLine)
			{
				_choicesByLine[entry.Key] = new List<DialogueChoice>(entry.Value);
			}
		}

		_currentLineIndex = 0;
		_isDialogueRunning = true;
		_dialogueFinished = false;
		_waitingForChoice = false;
		_inputLocked = false;

		HideChoices();
		Visible = true;
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

	private void ShowCurrentLine()
	{
		HideChoices();

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

		if (_choicesByLine.TryGetValue(_currentLineIndex, out List<DialogueChoice> choices))
		{
			ShowChoices(choices);
		}
	}

	private void ShowChoices(List<DialogueChoice> choices)
	{
		_waitingForChoice = true;
		_currentChoices.Clear();
		_currentChoices.AddRange(choices);

		Button[] buttons =
		{
			_choiceButton1,
			_choiceButton2,
			_choiceButton3
		};

		for (int i = 0; i < buttons.Length; i++)
		{
			bool hasChoice = i < _currentChoices.Count;

			buttons[i].Visible = hasChoice;
			buttons[i].Disabled = !hasChoice;

			if (hasChoice)
			{
				buttons[i].Text = _currentChoices[i].Text;
			}
		}

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
		_choiceButton1.Visible = true;
		_choiceButton2.Visible = true;
		_choiceButton3.Visible = true;
		_choiceButton1.Disabled = false;
		_choiceButton2.Disabled = false;
		_choiceButton3.Disabled = false;
	}

	private void FinishDialogue()
	{
		if (_dialogueFinished)
		{
			return;
		}

		_dialogueFinished = true;
		_isDialogueRunning = false;
		_waitingForChoice = false;
		HideChoices();

		EmitSignal(SignalName.DialogueFinished);
	}

	public override void _Input(InputEvent @event)
	{
		if (!Visible ||
			!_isDialogueRunning ||
			_dialogueFinished ||
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
