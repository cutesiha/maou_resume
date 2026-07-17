using Godot;

/// <summary>
/// MainScene의 날짜 표시와 테스트 버튼을 관리합니다.
/// </summary>
public partial class MainSceneController : Control
{
	private Label _dayLabel = null!;
	private Button _advanceDayButton = null!;
	private Button _printStateButton = null!;

	private GameManager _gameManager = null!;

	public override void _Ready()
	{
		// 오토로드로 등록된 GameManager를 가져옵니다.
		_gameManager = GetNode<GameManager>("/root/GameManager");

		// MainScene 안의 UI 노드를 가져옵니다.
		_dayLabel = GetNode<Label>(
            "CenterContainer/VBoxContainer/DayLabel"
		);

		_advanceDayButton = GetNode<Button>(
            "CenterContainer/VBoxContainer/AdvanceDayButton"
		);

		_printStateButton = GetNode<Button>(
            "CenterContainer/VBoxContainer/PrintStateButton"
		);

		// 버튼 신호를 함수와 연결합니다.
		_advanceDayButton.Pressed += OnAdvanceDayPressed;
		_printStateButton.Pressed += OnPrintStatePressed;

		RefreshDayDisplay();
	}

	private void OnAdvanceDayPressed()
	{
		bool advanced = _gameManager.AdvanceDay();

		RefreshDayDisplay();

		if (!advanced)
		{
			_dayLabel.Text = "현재 날짜: 7일차 (마지막 날)";
		}
	}

	private void OnPrintStatePressed()
	{
		_gameManager.PrintCurrentState();
	}

	private void RefreshDayDisplay()
	{
		int currentDay = _gameManager.CurrentState.Day;

		if (_gameManager.IsFinalDay())
		{
			_dayLabel.Text = $"현재 날짜: {currentDay}일차 (마지막 날)";
			return;
		}

		_dayLabel.Text = $"현재 날짜: {currentDay}일차";
	}
}
