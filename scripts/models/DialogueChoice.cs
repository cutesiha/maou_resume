using Godot;

/// <summary>
/// 대화 선택지 한 개의 정보를 저장합니다.
/// </summary>
public partial class DialogueChoice : RefCounted
{
	public string Text { get; set; }
	public int AffectionChange { get; set; }
	public string FlagId { get; set; }
	public DialogueLine ResultLine { get; set; }

	public DialogueChoice(
		string text,
		int affectionChange,
		string flagId,
		DialogueLine resultLine
	)
	{
		Text = text;
		AffectionChange = affectionChange;
		FlagId = flagId;
		ResultLine = resultLine;
	}
}
