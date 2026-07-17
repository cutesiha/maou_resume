using Godot;
using Godot.Collections;

/// <summary>
/// 현재 플레이의 진행 상황을 보관합니다.
/// 나중에 날짜, 호감도, 경력 기록, 스토리 플래그 등이 추가됩니다.
/// </summary>
[GlobalClass]
public partial class GameState : Resource
{
	[Export(PropertyHint.Range, "1,7,1")]
	public int Day { get; set; } = 1;

	[Export(PropertyHint.Range, "0,10,1")]
	public int JaeyoonAffection { get; set; } = 0;

	[Export]
	public string SelectedMorningId { get; set; } = "";

	[Export]
	public string SelectedAfternoonId { get; set; } = "";

	[Export]
	public Array<TraitGroupData> TraitGroups { get; set; } = new();

	/// <summary>
	/// 새 게임에 사용할 초기 상태를 생성합니다.
	/// </summary>
	public static GameState CreateNewGame()
	{
		GameState state = new()
		{
			Day = 1,
			JaeyoonAffection = 0,
			SelectedMorningId = "",
			SelectedAfternoonId = ""
		};

		state.TraitGroups.Add(
			TraitGroupData.CreateInitial(
				"TR_O01",
				"오만",
				"자신감",
                "자기표현"
			)
		);

		state.TraitGroups.Add(
			TraitGroupData.CreateInitial(
				"TR_V01",
				"폭력성",
				"결단력",
                "보호본능"
			)
		);

		state.TraitGroups.Add(
			TraitGroupData.CreateInitial(
				"TR_C01",
				"지배욕",
				"리더십",
                "책임감"
			)
		);

		state.TraitGroups.Add(
			TraitGroupData.CreateInitial(
				"TR_L01",
				"게으름",
				"효율성",
                "침착함"
			)
		);

		state.TraitGroups.Add(
			TraitGroupData.CreateInitial(
				"TR_D01",
				"인간 불신",
				"경계심",
                "신중함"
			)
		);

		return state;
	}

	/// <summary>
	/// 5개 계열의 데이터가 모두 올바른지 검사합니다.
	/// </summary>
	public bool AreAllTraitGroupsValid()
	{
		if (TraitGroups.Count != 5)
		{
			return false;
		}

		foreach (TraitGroupData group in TraitGroups)
		{
			if (group is null || !group.IsValid())
			{
				return false;
			}
		}

		return true;
	}
}
