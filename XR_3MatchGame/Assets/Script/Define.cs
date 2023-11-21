namespace XR_3MatchGame.Util
{
    public enum SceneType
    {
        None, Main, InGame, Lobby, Select, Loading, FirstLoading
    }

    public enum PoolType
    {
        None, Block
    }

    public enum GameState
    {
        None, Play, Checking, Skill, End
    }

    public enum AtlasType
    {
        None, BlockAtlas, IconAtlas, ClockAtlas
    }

    public enum SwipeDir
    {
        None, Top, Bottom, Left, Right
    }

    public enum ElementType
    {
        None, Fire, Ice, Grass, Dark, Light, Lightning, Balance
    }

    public enum IntroPhase
    {
        None, Start, Resource, Data, UI, Complete
    }

    public enum ItemType 
    {
        None, Boom, Time, Skill
    }

    public class StringName
    {
        public const string HighScore_0 = "HighScore_0";
        public const string HighScore_1 = "HighScore_1";
    }

}