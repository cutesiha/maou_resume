using Godot;

/// <summary>
/// Handles the temporary title menu.
/// </summary>
public partial class TitleSceneController : Control
{
	private const string PrologueScenePath = "res://scenes/prologue/prologue_scene.tscn";

	private Label _titleLabel = null!;
	private Button _newGameButton = null!;
	private Button _continueButton = null!;
	private Button _exitButton = null!;
	private ConfirmationDialog _exitConfirmationDialog = null!;
	private GameManager _gameManager = null!;

	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");

		if (!TryFindRequiredNodes())
		{
			return;
		}

		ConfigureUi();
		ConnectSignals();
	}

	private bool TryFindRequiredNodes()
	{
		bool allFound = true;

		_titleLabel = GetNodeOrNull<Label>(
			"CenterContainer/VBoxContainer/TitleLabel"
		);
		_newGameButton = GetNodeOrNull<Button>(
			"CenterContainer/VBoxContainer/NewGameButton"
		);
		_continueButton = GetNodeOrNull<Button>(
			"CenterContainer/VBoxContainer/ContinueButton"
		);
		_exitButton = GetNodeOrNull<Button>(
			"CenterContainer/VBoxContainer/ExitButton"
		);
		_exitConfirmationDialog = GetNodeOrNull<ConfirmationDialog>(
			"ExitConfirmationDialog"
		);

		allFound &= ReportMissingNode(
			_titleLabel,
			"CenterContainer/VBoxContainer/TitleLabel"
		);
		allFound &= ReportMissingNode(
			_newGameButton,
			"CenterContainer/VBoxContainer/NewGameButton"
		);
		allFound &= ReportMissingNode(
			_continueButton,
			"CenterContainer/VBoxContainer/ContinueButton"
		);
		allFound &= ReportMissingNode(
			_exitButton,
			"CenterContainer/VBoxContainer/ExitButton"
		);
		allFound &= ReportMissingNode(
			_exitConfirmationDialog,
			"ExitConfirmationDialog"
		);

		return allFound;
	}

	private static bool ReportMissingNode(Node node, string nodePath)
	{
		if (node is not null)
		{
			return true;
		}

		GD.PushError($"TitleScene: 필수 노드를 찾을 수 없습니다. 경로: {nodePath}");
		return false;
	}

	private void ConfigureUi()
	{
		_titleLabel.Text = "마왕님, 자소서부터 쓰세요!";
		_newGameButton.Text = "새 게임";
		_continueButton.Text = "이어하기";
		_exitButton.Text = "종료";

		_continueButton.Disabled = true;
		_continueButton.TooltipText = "저장 데이터가 없습니다.";

		_exitConfirmationDialog.Title = "";
		_exitConfirmationDialog.DialogText = "정말로 게임을 종료하시겠습니까?";
		_exitConfirmationDialog.OkButtonText = "예";
		_exitConfirmationDialog.CancelButtonText = "아니오";
	}

	private void ConnectSignals()
	{
		_newGameButton.Pressed += OnNewGamePressed;
		_exitButton.Pressed += OnExitPressed;
		_exitConfirmationDialog.Confirmed += OnExitConfirmed;
	}

	private void OnNewGamePressed()
	{
		_gameManager.StartNewGame();

		if (!ResourceLoader.Exists(PrologueScenePath))
		{
			GD.PushError($"TitleScene: 씬 경로가 존재하지 않습니다. {PrologueScenePath}");
			return;
		}

		_gameManager.ChangeScene(PrologueScenePath);
	}

	private void OnExitPressed()
	{
		_exitConfirmationDialog.PopupCentered();
	}

	private void OnExitConfirmed()
	{
		GetTree().Quit();
	}
}
