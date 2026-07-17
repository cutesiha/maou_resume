using Godot;

/// <summary>
/// Plays the temporary boot logo sequence, then opens the title scene.
/// </summary>
public partial class BootSceneController : Control
{
	private const string TitleScenePath = "res://scenes/title/title_scene.tscn";
	private const double FadeInDuration = 0.5;
	private const double HoldDuration = 1.0;
	private const double FadeOutDuration = 0.5;

	private Label _logoLabel = null!;
	private GameManager _gameManager = null!;
	private Tween _logoTween = null!;
	private bool _sequenceStarted = false;

	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>("/root/GameManager");

		if (!TryFindRequiredNodes())
		{
			return;
		}

		StartBootSequence();
	}

	private bool TryFindRequiredNodes()
	{
		_logoLabel = GetNodeOrNull<Label>("LogoContainer/LogoLabel");

		if (_logoLabel is null)
		{
			GD.PushError(
				"BootScene: 필수 노드를 찾을 수 없습니다. 경로: LogoContainer/LogoLabel"
			);
			return false;
		}

		return true;
	}

	private void StartBootSequence()
	{
		if (_sequenceStarted)
		{
			return;
		}

		if (_logoTween is not null && _logoTween.IsRunning())
		{
			return;
		}

		_sequenceStarted = true;

		Color logoColor = _logoLabel.Modulate;
		logoColor.A = 0.0f;
		_logoLabel.Modulate = logoColor;

		_logoTween = CreateTween();
		_logoTween.TweenProperty(
			_logoLabel,
			"modulate:a",
			1.0,
			FadeInDuration
		);
		_logoTween.TweenInterval(HoldDuration);
		_logoTween.TweenProperty(
			_logoLabel,
			"modulate:a",
			0.0,
			FadeOutDuration
		);
		_logoTween.TweenCallback(Callable.From(GoToTitleScene));
	}

	private void GoToTitleScene()
	{
		if (!ResourceLoader.Exists(TitleScenePath))
		{
			GD.PushError($"BootScene: 씬 경로가 존재하지 않습니다. {TitleScenePath}");
			return;
		}

		_gameManager.ChangeScene(TitleScenePath);
	}
}
