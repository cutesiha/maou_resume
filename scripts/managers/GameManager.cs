using Godot;

/// <summary>
/// 게임의 현재 상태와 전체 진행을 관리합니다.
/// 오토로드로 등록하여 씬이 바뀌어도 유지합니다.
/// </summary>
public partial class GameManager : Node
{
	public const int FirstDay = 1;
	public const int LastDay = 7;

	/// <summary>
	/// 현재 플레이 중인 게임 상태입니다.
	/// </summary>
	public GameState CurrentState { get; private set; } = null!;

	public override void _Ready()
	{
		StartNewGame();

		GD.Print("GameManager가 준비되었습니다.");
		PrintCurrentState();
	}

	/// <summary>
	/// 모든 데이터를 초기값으로 되돌리고 새 게임을 시작합니다.
	/// </summary>
	public void StartNewGame()
	{
		CurrentState = GameState.CreateNewGame();

		GD.Print("새 게임 상태를 생성했습니다.");
	}

	/// <summary>
	/// 현재 날짜를 하루 증가시킵니다.
	/// 7일차를 넘을 수 없습니다.
	/// </summary>
	public bool AdvanceDay()
	{
		if (CurrentState.Day >= LastDay)
		{
			GD.Print("이미 마지막 날입니다.");
			return false;
		}

		CurrentState.Day++;

		// 새 날짜가 시작되면 전날의 일정 선택값을 초기화합니다.
		CurrentState.SelectedMorningId = "";
		CurrentState.SelectedAfternoonId = "";

		GD.Print($"{CurrentState.Day}일차로 진행했습니다.");
		return true;
	}

	/// <summary>
	/// 현재 날짜가 최종일인지 확인합니다.
	/// </summary>
	public bool IsFinalDay()
	{
		return CurrentState.Day == LastDay;
	}

	/// <summary>
	/// 현재 진행 상태를 출력창에 표시합니다.
	/// </summary>
	public void PrintCurrentState()
	{
		GD.Print("===== 현재 게임 상태 =====");
		GD.Print($"날짜: {CurrentState.Day}일차");
		GD.Print($"재윤 호감도: {CurrentState.JaeyoonAffection}");
		GD.Print($"오전 일정: {GetDisplayId(CurrentState.SelectedMorningId)}");
		GD.Print($"오후 일정: {GetDisplayId(CurrentState.SelectedAfternoonId)}");
		GD.Print("==========================");
	}

	/// <summary>
	/// 빈 일정 ID를 출력할 때 보기 좋은 문구로 바꿉니다.
	/// </summary>
	private static string GetDisplayId(string id)
	{
		return string.IsNullOrWhiteSpace(id) ? "선택 안 함" : id;
	}

	/// <summary>
	/// 파일 경로를 사용하여 지정한 씬으로 이동합니다.
	/// </summary>
	public void ChangeScene(string scenePath)
	{
		if (string.IsNullOrWhiteSpace(scenePath))
		{
			GD.PushError("이동할 씬 경로가 비어 있습니다.");
			return;
		}

		Error result = GetTree().ChangeSceneToFile(scenePath);

		if (result != Error.Ok)
		{
			GD.PushError($"씬 전환 실패: {scenePath} / 오류: {result}");
		}
	}
	
	/// <summary>
	/// 재윤 호감도를 변경합니다.
	/// 호감도는 0보다 작아지거나 10보다 커지지 않습니다.
	/// </summary>
	public void ChangeJaeyoonAffection(int amount)
	{
		int before = CurrentState.JaeyoonAffection;

		CurrentState.JaeyoonAffection = Mathf.Clamp(
			CurrentState.JaeyoonAffection + amount,
			0,
			10
		);

		GD.Print(
			$"재윤 호감도: {before} → " +
			$"{CurrentState.JaeyoonAffection}"
		);
	}

	/// <summary>
	/// 이벤트 플래그를 현재 게임 상태에 추가합니다.
	/// </summary>
	public void AddFlag(string flagId)
	{
		CurrentState.AddFlag(flagId);
		GD.Print($"플래그 획득: {flagId}");
	}
}
