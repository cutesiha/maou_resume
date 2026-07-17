using Godot;

public partial class DebugSceneButton : Button
{
	[Export(PropertyHint.File, "*.tscn")]
	public string TargetScenePath { get; set; } = "";

	public override void _Ready()
	{
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		if (string.IsNullOrWhiteSpace(TargetScenePath))
		{
			GD.PushError($"{Name}: 이동할 씬 경로가 설정되지 않았습니다.");
			return;
		}

		Error result = GetTree().ChangeSceneToFile(TargetScenePath);

		if (result != Error.Ok)
		{
			GD.PushError(
				$"{Name}: 씬 전환 실패 - {TargetScenePath} / 오류: {result}"
			);
		}
	}
}
