using MapChanger.UI;

namespace ArchipelagoMapMod.UI;

internal class apmmTitle : Title
{
    private string _hoveredText;

    public apmmTitle() : base(ArchipelagoMapMod.MOD)
    {
        Instance = this;
    }

    internal static apmmTitle Instance { get; private set; }

    internal string HoveredText
    {
        get => _hoveredText;
        set
        {
            _hoveredText = value;
            Update();
        }
    }

    public override void Update()
    {
        base.Update();

        if (_hoveredText is not null) TitleText.Text = _hoveredText;

        TitleText.ContentColor = apmmColors.GetColor(apmmColorSetting.UI_Neutral);
    }
}