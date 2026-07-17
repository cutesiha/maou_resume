using Godot;
using System.Collections.Generic;

/// <summary>
/// Test host for the reusable dialogue panel.
/// </summary>
public partial class DialogueSceneController : Control
{
	private DialoguePanelController _dialoguePanel = null!;
	private GameManager _gameManager = null!;

	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");

		_dialoguePanel = GetNode<DialoguePanelController>("DialoguePanel");
		_dialoguePanel.DialogueFinished += OnDialogueFinished;

		_dialoguePanel.StartDialogue(
			CreateSampleDialogue(),
			CreateSampleChoices()
		);
	}

	private static List<DialogueLine> CreateSampleDialogue()
	{
		return new List<DialogueLine>
		{
			new(
				"재윤",
				"오늘은 자소서에 쓸 내용을 정리하겠습니다.",
				"Serious"
			),
			new(
				"벨",
				"마계를 정복한 위업을 첫 줄에 적어라.",
				"Proud"
			),
			new(
				"재윤",
				"그 전에 묻겠습니다. 제 말을 조금이라도 믿습니까?",
				"Neutral"
			),
			new(
				"재윤",
				"그럼 다음 내용을 계속 정리하죠.",
				"Serious"
			),
			new(
				"벨",
				"좋다. 인간의 문서 작성법을 정복해 주마.",
				"Proud"
			)
		};
	}

	private static Dictionary<int, List<DialogueChoice>> CreateSampleChoices()
	{
		return new Dictionary<int, List<DialogueChoice>>
		{
			[2] = new List<DialogueChoice>
			{
				new(
					"믿는다",
					1,
					"TRUST_JAEYOON",
					new DialogueLine(
						"재윤",
						"그 말은 기억해 두겠습니다.",
						"Surprised"
					)
				),
				new(
					"아직 모르겠다",
					0,
					"UNSURE_JAEYOON",
					new DialogueLine(
						"재윤",
						"당장 대답하기 어렵다는 건 이해합니다.",
						"Neutral"
					)
				),
				new(
					"믿지 않는다",
					-1,
					"DISTRUST_JAEYOON",
					new DialogueLine(
						"재윤",
						"솔직한 대답이군요. 예상은 했습니다.",
						"Tired"
					)
				)
			}
		};
	}

	private void OnDialogueFinished()
	{
		GD.Print("DialogueScene dialogue panel test completed.");
		PrintFlags();
	}

	private void PrintFlags()
	{
		GD.Print("===== 현재 이벤트 플래그 =====");

		foreach (string flag in _gameManager.CurrentState.EventFlags)
		{
			GD.Print(flag);
		}

		GD.Print("============================");
	}
}
