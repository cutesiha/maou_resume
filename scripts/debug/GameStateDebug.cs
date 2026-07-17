using Godot;

/// <summary>
/// 3일차 초기 데이터가 올바르게 만들어지는지 검사하는 임시 스크립트입니다.
/// </summary>
public partial class GameStateDebug : Node
{
	public override void _Ready()
	{
		GameState gameState = GameState.CreateNewGame();

		GD.Print("===== 새 게임 초기 데이터 확인 =====");
		GD.Print($"현재 날짜: {gameState.Day}일차");
		GD.Print($"재윤 호감도: {gameState.JaeyoonAffection}");
		GD.Print($"성향 계열 개수: {gameState.TraitGroups.Count}");
		GD.Print("-----------------------------------");

		foreach (TraitGroupData group in gameState.TraitGroups)
		{
			GD.Print(group.GetSummary());

			if (!group.IsValid())
			{
				GD.PushError(
					$"{group.GroupId}의 수치가 잘못되었습니다. " +
					$"현재 총합: {group.GetTotal()}"
				);
			}
		}

		GD.Print("-----------------------------------");

		if (gameState.AreAllTraitGroupsValid())
		{
			GD.Print("검증 성공: 5개 성향 계열이 모두 정상입니다.");
		}
		else
		{
			GD.PushError("검증 실패: 초기 성향 데이터를 확인하세요.");
		}

		GD.Print("===================================");
	}
}
