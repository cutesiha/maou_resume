using Godot;

/// <summary>
/// 대화 한 줄의 정보를 저장합니다.
/// </summary>
public partial class DialogueLine : RefCounted
{
	public string SpeakerName { get; set; }
	public string BodyText { get; set; }
	public string Expression { get; set; }

	public DialogueLine(
		string speakerName,
		string bodyText,
		string expression
	)
	{
		SpeakerName = speakerName;
		BodyText = bodyText;
		Expression = expression;
	}
}
