using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_Resource;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_UI
{
    public class SkillButton : MonoBehaviour
    {
        public ElementType selectElement;   // 선택한 원소

        [SerializeField]
        private Image icon;

        [SerializeField]
        private GameObject skillEffectObj;      // 큰놈

        [SerializeField]
        private ParticleSystem magicCircle;     // 작은놈 1

        [SerializeField]
        private ParticleSystem magicEffect;     // 작은놈 2

        [SerializeField]
        private GameObject backBlack;           // 스킬 사용 시 나올 검은 배경

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 초기 세팅 메서드
        /// </summary>
        public void Initialize()
        {
            var GM = GameManager.Instance;

            var index = int.Parse(name.Split('_')[1]);

            switch (index)
            {
                case 0:
                    selectElement = GM.selectType[0];

                    for (int i = 0; i < skillEffectObj.transform.childCount; i++)
                    {
                        var name = skillEffectObj.transform.GetChild(i).name.Split('_')[0];

                        if (selectElement.ToString() == name)
                        {
                            var obj = skillEffectObj.transform.GetChild(i).gameObject;

                            magicCircle = obj.transform.GetChild(0).GetComponent<ParticleSystem>();
                            magicEffect = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
                        }
                    }

                    break;

                case 1:
                    selectElement = GM.selectType[1];

                    for (int i = 0; i < skillEffectObj.transform.childCount; i++)
                    {
                        var name = skillEffectObj.transform.GetChild(i).name.Split('_')[0];

                        if (selectElement.ToString() == name)
                        {
                            var obj = skillEffectObj.transform.GetChild(i).gameObject;

                            magicCircle = obj.transform.GetChild(0).GetComponent<ParticleSystem>();
                            magicEffect = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
                        }
                    }

                    break;

                case 2:
                    selectElement = GM.selectType[2];

                    for (int i = 0; i < skillEffectObj.transform.childCount; i++)
                    {
                        var name = skillEffectObj.transform.GetChild(i).name.Split('_')[0];

                        if (selectElement.ToString() == name)
                        {
                            var obj = skillEffectObj.transform.GetChild(i).gameObject;

                            magicCircle = obj.transform.GetChild(0).GetComponent<ParticleSystem>();
                            magicEffect = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
                        }
                    }

                    break;
            }



            icon.sprite = SpriteLoader.GetSprite(AtlasType.IconAtlas, selectElement.ToString());
        }

        public void OnSkill()
        {
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            // 현재 게임 상태가  Play 상태일때만 스킬 사용 가능
            if (uiElement.GetGauge() >= 1f && GameManager.Instance.GameState == GameState.Play)
            {
                // 검은 배경 활성화
                backBlack.SetActive(true);

                Debug.Log("스킬을 사용합니다!");

                // 스킬 게이지 0으로 초기화
                uiElement.Initialize();

                var GM = GameManager.Instance;
                GM.SetGameState(GameState.SKill);

                // 스킬을 시작합니다.
                StartCoroutine(StartSkill());
            }
        }

        private IEnumerator StartSkill()
        {
            var GM = GameManager.Instance;

            var blocks = GM.Board.blocks;
            var delBlocks = GM.Board.delBlocks;
            var downBlocks = GM.Board.downBlocks;
            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            switch (selectElement)
            {
                case ElementType.Fire:

                    // 첫번째 이펙트 실행
                    magicCircle.Play();

                    yield return new WaitForSeconds(1.5f);

                    // 두번째 이펙트 실행
                    magicEffect.Play();

                    // 파괴할 블럭 찾기
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row == 2 || blocks[i].row == 3 || blocks[i].row == 4)
                            {
                                delBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    // 검은 배경 비활성화
                    backBlack.SetActive(false);

                    // 블럭 파티클 실행
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        delBlocks[i].BlockParticle();
                    }

                    // 블럭 파괴

                    yield return new WaitForSeconds(.3f);

                    // 블럭 파괴
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        blockPool.ReturnPoolableObject(delBlocks[i]);
                        blocks.Remove(delBlocks[i]);

                        DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                    }

                    // 내일 블럭 찾기
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row > 4)
                            {
                                downBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    yield return new WaitForSeconds(.4f);

                    // 블럭 내리기
                    // 블럭을 내리는 작업
                    for (int i = 0; i < downBlocks.Count; i++)
                    {
                        var targetRow = (downBlocks[i].row -= 3);

                        if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                        {
                            Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                            downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                        }
                    }

                    // 빈자리에 블럭 채워넣기
                    for (int row = 4; row < GM.BoardSize.y; row++)
                    {
                        for (int col = 2; col < 5; col++)
                        {
                            var block = blockPool.GetPoolableObject(obj => obj.CanRecycle);
                            block.transform.position = new Vector3(col, 7, 0);
                            block.Initialize(col, 7);
                            block.gameObject.SetActive(true);

                            blocks.Add(block);

                            // 위에서 아래로 내려가는 것처럼
                            var targetRow = (block.row = row);

                            if (Mathf.Abs(targetRow - block.transform.position.y) > .1f)
                            {
                                Vector2 tempPosition = new Vector2(block.transform.position.x, targetRow);
                                block.transform.position = Vector2.Lerp(block.transform.position, tempPosition, .05f);
                            }
                        }

                        yield return new WaitForSeconds(.3f);
                    }

                    delBlocks.Clear();
                    downBlocks.Clear();

                    uiElement.SetStateText("일정 시간동안 추가 점수!");

                    GM.isPlus = true;

                    // 블럭 업데이트
                    //GM.Board.BlockUpdate();

                    if (GM.Board.BlockCheck())
                    {
                        GM.isStart = true;
                        GM.SetGameState(GameState.Checking);
                    }
                    else
                    {
                        GM.SetGameState(GameState.Play);
                    }

                    yield return new WaitForSeconds(5f);

                    GM.isPlus = false;
                    uiElement.SetStateText();

                    break;

                case ElementType.Ice:

                    // 첫번째 이펙트 실행
                    magicCircle.Play();

                    yield return new WaitForSeconds(1.5f);

                    // 두번째 이펙트 실행
                    magicEffect.Play();

                    // 파괴할 블럭 찾기
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row == 2 || blocks[i].row == 3 || blocks[i].row == 4)
                            {
                                delBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    // 검은 배경 비활성화
                    backBlack.SetActive(false);

                    // 블럭 파티클 실행
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        delBlocks[i].BlockParticle();
                    }

                    yield return new WaitForSeconds(.3f);

                    // 블럭 파괴
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        blockPool.ReturnPoolableObject(delBlocks[i]);
                        blocks.Remove(delBlocks[i]);

                        DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                    }

                    // 내일 블럭 찾기
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row > 4)
                            {
                                downBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    yield return new WaitForSeconds(.4f);

                    // 블럭을 내리는 작업
                    for (int i = 0; i < downBlocks.Count; i++)
                    {
                        var targetRow = (downBlocks[i].row -= 3);

                        if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                        {
                            Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                            downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                        }
                    }

                    // 빈자리에 블럭 채워넣기
                    for (int row = 4; row < GM.BoardSize.y; row++)
                    {
                        for (int col = 2; col < 5; col++)
                        {
                            var block = blockPool.GetPoolableObject(obj => obj.CanRecycle);
                            block.transform.position = new Vector3(col, 7, 0);
                            block.Initialize(col, 7);
                            block.gameObject.SetActive(true);

                            blocks.Add(block);

                            // 위에서 아래로 내려가는 것처럼
                            var targetRow = (block.row = row);

                            if (Mathf.Abs(targetRow - block.transform.position.y) > .1f)
                            {
                                Vector2 tempPosition = new Vector2(block.transform.position.x, targetRow);
                                block.transform.position = Vector2.Lerp(block.transform.position, tempPosition, .05f);
                            }
                        }

                        yield return new WaitForSeconds(.3f);
                    }

                    uiElement.SetStateText("일정 시간동안 시간 정지!");
                    UIWindowManager.Instance.GetWindow<UITime>().timeStop = true;

                    delBlocks.Clear();
                    downBlocks.Clear();

                    // 블럭 업데이트
                    //GM.Board.BlockUpdate();

                    if (GM.Board.BlockCheck())
                    {
                        GM.isStart = true;
                        GM.SetGameState(GameState.Checking);
                    }
                    else
                    {
                        GM.SetGameState(GameState.Play);
                    }

                    yield return new WaitForSeconds(10f);

                    uiElement.SetStateText();
                    UIWindowManager.Instance.GetWindow<UITime>().timeStop = false;

                    break;

                case ElementType.Grass:

                    // 첫번째 이펙트 실행
                    magicCircle.Play();

                    yield return new WaitForSeconds(1.5f);

                    // 두번째 이펙트 실행
                    magicEffect.Play();

                    // 파괴할 블럭 찾기
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row == 2 || blocks[i].row == 3 || blocks[i].row == 4)
                            {
                                delBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    // 검은 배경 비활성화
                    backBlack.SetActive(false);

                    // 블럭 파티클 실행
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        delBlocks[i].BlockParticle();
                    }

                    yield return new WaitForSeconds(.3f);

                    // 블럭 파괴
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        blockPool.ReturnPoolableObject(delBlocks[i]);
                        blocks.Remove(delBlocks[i]);

                        DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                    }

                    // 내일 블럭 찾기
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row > 4)
                            {
                                downBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    yield return new WaitForSeconds(.4f);

                    // 블럭 내리기
                    // 블럭을 내리는 작업
                    for (int i = 0; i < downBlocks.Count; i++)
                    {
                        var targetRow = (downBlocks[i].row -= 3);

                        if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                        {
                            Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                            downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                        }
                    }


                    // 빈자리에 블럭 채워넣기
                    for (int row = 4; row < GM.BoardSize.y; row++)
                    {
                        for (int col = 2; col < 5; col++)
                        {
                            var block = blockPool.GetPoolableObject(obj => obj.CanRecycle);
                            block.transform.position = new Vector3(col, 7, 0);
                            block.Initialize(col, 7);
                            block.gameObject.SetActive(true);

                            blocks.Add(block);

                            // 위에서 아래로 내려가는 것처럼
                            var targetRow = (block.row = row);

                            if (Mathf.Abs(targetRow - block.transform.position.y) > .1f)
                            {
                                Vector2 tempPosition = new Vector2(block.transform.position.x, targetRow);
                                block.transform.position = Vector2.Lerp(block.transform.position, tempPosition, .05f);
                            }
                        }

                        yield return new WaitForSeconds(.3f);
                    }

                    uiElement.SetStateText("일정 시간동안 스킬 게이지 회복!");

                    delBlocks.Clear();
                    downBlocks.Clear();

                    for (int i = 0; i < 9; i++)
                    {
                        uiElement.SetGauge(.05f);
                        yield return new WaitForSeconds(.3f);
                    }

                    // 블럭 업데이트
                    //GM.Board.BlockUpdate();

                    if (GM.Board.BlockCheck())
                    {
                        GM.isStart = true;
                        GM.SetGameState(GameState.Checking);
                    }
                    else
                    {
                        GM.SetGameState(GameState.Play);
                    }

                    break;
            }

            // 한번더 체크
            //GM.Board.BlockUpdate();

            if (GM.Board.BlockCheck())
            {
                GM.isStart = true;
                GM.SetGameState(GameState.Checking);
            }
            else
            {
                GM.SetGameState(GameState.Play);
            }
        }

    }
}