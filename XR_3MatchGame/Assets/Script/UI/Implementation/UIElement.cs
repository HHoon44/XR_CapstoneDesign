using System.Collections;
using UIHealthAlchemy;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_UI
{
    public class UIElement : UIWindow
    {
        #region GaugeObj
        [SerializeField]
        private GameObject fireGauge;            // 불 원소 스킬 게이지

        [SerializeField]
        private GameObject iceGauge;             // 얼음 원소 스킬 게이지

        [SerializeField]
        private GameObject grassGauge;           // 풀 원소 스킬 게이지

        #endregion

        #region SkillObj

        [SerializeField]
        private GameObject fireSkill;         // 스킬 컷씬

        [SerializeField]
        private GameObject iceSkill;         // 스킬 컷씬

        [SerializeField]
        private GameObject grassSkill;         // 스킬 컷씬

        #endregion

        private MaterialHealhBar skillGauge;
        private GameManager GM;

        private float skillValue;           // 스킬 게이지 값

        public override void Start()
        {
            base.Start();

            GM = GameManager.Instance;

            // 원소 게이지 활성화
            OnElementGauge(GM.ElementType);

            skillValue = 0;
        }

        /// <summary>
        /// 요청 받은 원소 타입에 따라 스킬 게이지를 활성화 해주는 메서드
        /// </summary>
        /// <param name="type">원소 타입</param>
        public void OnElementGauge(ElementType type)
        {
            switch (type)
            {
                case ElementType.Fire:

                    // 불 스킬 게이지 활성화
                    fireGauge.SetActive(true);

                    if (iceGauge.activeSelf)
                    {
                        iceGauge.SetActive(false);
                    }
                    else if (grassGauge.activeSelf)
                    {
                        grassGauge.SetActive(false);
                    }

                    skillGauge = fireGauge.GetComponent<MaterialHealhBar>();
                    skillGauge.Value = skillValue;
                    break;

                case ElementType.Ice:

                    // 얼음 스킬 게이지 활성화
                    iceGauge.SetActive(true);

                    if (fireGauge.activeSelf)
                    {
                        fireGauge.SetActive(false);
                    }
                    else if (grassGauge.activeSelf)
                    {
                        grassGauge.SetActive(false);
                    }

                    skillGauge = iceGauge.GetComponent<MaterialHealhBar>();
                    skillGauge.Value = skillValue;
                    break;

                case ElementType.Grass:

                    // 풀 스킬 게이지 활성화
                    grassGauge.SetActive(true);

                    if (fireGauge.activeSelf)
                    {
                        fireGauge.SetActive(false);
                    }
                    else if (iceGauge.activeSelf)
                    {
                        iceGauge.SetActive(false);
                    }

                    skillGauge = grassGauge.GetComponent<MaterialHealhBar>();
                    skillGauge.Value = skillValue;
                    break;
            }
        }

        /// <summary>
        /// 스킬 게이지 값을 조절하는 메서드
        /// </summary>
        /// <param name="value">값</param>
        public void SetSkillAmount(float value)
        {
            if (skillGauge.Value >= 1f)
            {
                skillGauge.Value = 1f;
                return;
            }

            skillValue += value;
            skillGauge.Value += skillValue;
        }

        /// <summary>
        /// 스킬 버튼에 바인딩할 메서드
        /// </summary>
        public void SkillBtn()
        {
            // 스킬 시전
            if (skillGauge.Value >= 1f && GM.GameState == GameState.Play)
            {
                fireSkill.SetActive(true);
                switch (GM.ElementType)
                {
                    case ElementType.Fire:
                        fireSkill.SetActive(true);
                        break;

                    case ElementType.Ice:
                        iceSkill.SetActive(true);
                        break;

                    case ElementType.Grass:
                        grassSkill.SetActive(true);
                        break;

                    case ElementType.Dark:
                        break;

                    case ElementType.Light:
                        break;

                    case ElementType.Lightning:
                        break;
                }

                skillValue = 0;
                skillGauge.Value = skillValue;

                GM.SetGameState(GameState.Skill);

                StartCoroutine(SkillStart(GM.ElementType));
            }
        }

        private IEnumerator SkillStart(ElementType type)
        {
            var blocks = GM.Board.blocks;
            var downBlocks = GM.Board.downBlocks;
            var delBlocks = GM.Board.delBlocks;
            var pool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var size = (GM.BoardSize.x * GM.BoardSize.y);

            // 타입에 따라 스킬 발동
            switch (type)
            {
                case ElementType.Fire:
                    Debug.Log("불 원소 스킬 발동");

                    // 범위는 1 ~ 5
                    // 파괴할 블럭 찾기
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col != 0 && blocks[i].col != 6)
                        {
                            if (blocks[i].row != 0 && blocks[i].row != 6)
                            {
                                delBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    // 블럭 파괴
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        pool.ReturnPoolableObject(delBlocks[i]);
                        blocks.Remove(delBlocks[i]);
                    }

                    // 내릴 블럭 찾기
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col != 0 && blocks[i].col != 6)
                        {
                            if (blocks[i].row == 6)
                            {
                                downBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    yield return new WaitForSeconds(1f);

                    // 블럭 내리기
                    for (int i = 0; i < downBlocks.Count; i++)
                    {
                        var targetRow = (downBlocks[i].row = 1);

                        if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                        {
                            Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                            downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                        }
                    }

                    // 빈자리에 블럭 생성하기
                    var emptyCount = size - blocks.Count;
                    var colValue = 1;
                    var rowValue = downBlocks[downBlocks.Count - 1].row + 1;

                    for (int i = 0; i < emptyCount; i++)
                    {
                        if (rowValue < GM.BoardSize.y)
                        {
                            // 새로운 블럭 생성 로직 작성
                            var newBlock = pool.GetPoolableObject(obj => obj.CanRecycle);
                            newBlock.transform.position = new Vector3(colValue, GM.BoardSize.y, 0);
                            newBlock.gameObject.SetActive(true);
                            newBlock.Initialize(colValue, GM.BoardSize.y);
                            blocks.Add(newBlock);

                            // 위에서 아래로 내려가는 것처럼
                            var targetRow = (newBlock.row = rowValue);

                            if (Mathf.Abs(targetRow - newBlock.transform.position.y) > .1f)
                            {
                                Vector2 tempPosition = new Vector2(newBlock.transform.position.x, targetRow);
                                newBlock.transform.position = Vector2.Lerp(newBlock.transform.position, tempPosition, .05f);
                            }

                            colValue++;

                            if (colValue > 5)
                            {
                                colValue = 1;
                                rowValue++;
                            }
                        }
                    }

                    delBlocks.Clear();
                    downBlocks.Clear();
                    break;

                case ElementType.Ice:
                    Debug.Log("얼음 원소 스킬 발동");
                    UIWindowManager.Instance.GetWindow<UITime>().timeStop = true;
                    GM.SetGameState(GameState.Play);

                    // 10초정도 시간을 멈춘다
                    yield return new WaitForSeconds(10f);

                    UIWindowManager.Instance.GetWindow<UITime>().timeStop = false;
                    GM.SetGameState(GameState.Skill);
                    break;

                case ElementType.Grass:
                    Debug.Log("풀 원소 스킬 발동");
                    break;
            }

            yield return new WaitForSeconds(1f);

            fireSkill.SetActive(false);

            yield return new WaitForSeconds(.1f);

            // 블럭 업데이트
            GM.Board.BlockUpdate();

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