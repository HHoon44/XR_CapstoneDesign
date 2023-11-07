namespace XR_3MatchGame.Util
{
    public enum SceneType
    {
        None, Main, InGame, Lobby, Select
    }

    public enum PoolType
    {
        None, Block
    }

    public enum GameState
    {
        None, Play, Checking, End
    }

    public enum AtlasType
    {
        None, BlockAtlas
    }

    public enum BlockType
    {
        None, Dark, Fire, Light, Lightning, Tree, Ice
    }

    public enum SwipeDir
    {
        None, Top, Bottom, Left, Right
    }

    public enum BoomType
    {
        None, ColBoom, RowBoom
    }
}