using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;

namespace XR_3MatchGame_Util
{
    public class StageManager : Singleton<StageManager>
    {
        public string stageName;
        public ElementType stageType;

        public IEnumerator SpawnBlock()
        {
            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var size = GameManager.Instance.BoardSize;
            var blocks = GameManager.Instance.Board.blocks;

            yield return null;
        }
    }
}