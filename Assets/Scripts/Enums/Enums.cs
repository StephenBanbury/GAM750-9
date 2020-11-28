
namespace Assets.Scripts.Enums
{
    public enum MediaType
    {
        None = 0,
        VideoClip = 1,
        VideoStream = 2,
        Audio =3
    }

    public enum Source
    {
        LocalFile,
        Url
    }

    public enum ScreenFormation
    {
        None = 0,
        LargeSquare,
        SmallSquare,
        Cross,
        //SmallStar,
        LargeStar,
        Circle,
        Triangle,
        ShortRectangle,
        LongRectangle
    }

    public enum Scene
    {
        None = 0,
        Scene1,
        Scene2,
        Scene3,
        Scene4,
        Scene5,
        Scene6,
        Scene7,
        Scene8
    }

    public enum ScreenAction
    {
        None = 0,
        ChangeVideoClip,
        ChangeVideoStream,
        ChangeFormation,
        CreatePortal,
        DoTeleport
    }
}