using Godot;

/// <summary>
/// 하나의 성향 계열 데이터를 보관
/// 예: 오만 / 자신감 / 자기표현
/// </summary>
[GlobalClass]
public partial class TraitGroupData : Resource
{
	public const int RequiredTotal = 6;

	[Export]
	public string GroupId { get; set; } = "";

	[Export]
	public string DemonName { get; set; } = "";

	[Export]
	public string HumanAName { get; set; } = "";

	[Export]
	public string HumanBName { get; set; } = "";

	[Export(PropertyHint.Range, "0,6,1")]
	public int DemonValue { get; set; } = 6;

	[Export(PropertyHint.Range, "0,6,1")]
	public int HumanAValue { get; set; } = 0;

	[Export(PropertyHint.Range, "0,6,1")]
	public int HumanBValue { get; set; } = 0;

	/// <summary>
	/// 현재 세 성향 수치의 합계를 반환합니다.
	/// </summary>
	public int GetTotal()
	{
		return DemonValue + HumanAValue + HumanBValue;
	}

	/// <summary>
	/// 각 수치가 0~6 사이이고 합계가 6인지 검사합니다.
	/// </summary>
	public bool IsValid()
	{
		bool valuesAreInRange =
			DemonValue is >= 0 and <= 6 &&
			HumanAValue is >= 0 and <= 6 &&
			HumanBValue is >= 0 and <= 6;

		return valuesAreInRange && GetTotal() == RequiredTotal;
	}

	/// <summary>
	/// 초기 상태의 성향 계열을 생성합니다.
	/// </summary>
	public static TraitGroupData CreateInitial(
		string groupId,
		string demonName,
		string humanAName,
		string humanBName)
	{
		return new TraitGroupData
		{
			GroupId = groupId,
			DemonName = demonName,
			HumanAName = humanAName,
			HumanBName = humanBName,
			DemonValue = 6,
			HumanAValue = 0,
			HumanBValue = 0
		};
	}

	public string GetSummary()
	{
		return
			$"[{GroupId}] {DemonName} / {HumanAName} / {HumanBName}" +
			$" = {DemonValue} / {HumanAValue} / {HumanBValue}";
	}
}
