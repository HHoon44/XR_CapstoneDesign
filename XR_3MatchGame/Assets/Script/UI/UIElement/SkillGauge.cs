using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_UI;
using XR_3MatchGame_Util;

public class SkillGauge : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem circleEffect;

    [SerializeField]
    private ParticleSystem skillEffect;

    [SerializeField]
    private Image fillGauge;

    [SerializeField]
    private Animator anim;

    private GameManager GM;
    private DataManager DM;

    public bool isFill = false;

    public ElementType gaugeType;

    private void Awake()
    {
        var name = gameObject.name.Split('_')[0];

        switch (name)
        {
            case "Fire":
                gaugeType = ElementType.Fire;
                break;

            case "Ice":
                gaugeType = ElementType.Ice;
                break;

            case "Grass":
                gaugeType = ElementType.Grass;
                break;
        }
    }

    /// <summary>
    /// 스킬 버튼 초기화 메서드
    /// </summary>
    public void Initialize()
    {
        GM = GameManager.Instance;
        DM = DataManager.Instance;

        fillGauge.fillAmount = DM.saveValue;

        if (fillGauge.fillAmount >= 1f)
        {
            isFill = true;

            // 가득 찬 상태에서 원소를 바꿨을 경우
            // 변경된 아이콘도 애니메이션 실행되도록
            anim.SetBool("isFill", isFill);
        }
    }

    public void SetSkillAmount(float value)
    {
        if (fillGauge.fillAmount >= 1f)
        {
            DataManager.Instance.saveValue = 1f;
            fillGauge.fillAmount = DataManager.Instance.saveValue;

            if (!isFill)
            {
                isFill = true;
                anim.SetBool("isFill", isFill);
            }

            return;
        }

        DataManager.Instance.saveValue += value;
        fillGauge.fillAmount = DataManager.Instance.saveValue;
    }

    /// <summary>
    /// 스킬 버튼에 바인딩할 메서드
    /// </summary>
    public void SkillButton()
    {
        if (fillGauge.fillAmount >= 1f && GM.GameState == GameState.Play)
        {
            if (isFill)
            {
                isFill = false;
            }

            anim.SetBool("isFill", isFill);

            // 마법진 이펙트 실행
            circleEffect.Play();

            DataManager.Instance.saveValue = 0;
            fillGauge.fillAmount = 0;

            //switch (gaugeType)
            //{
            //    case ElementType.Fire:

            //        GM.SetGameState(GameState.FireSkill);
            //        break;

            //    case ElementType.Ice:

            //        GM.SetGameState(GameState.IceSkill);
            //        break;

            //    case ElementType.Grass:

            //        GM.SetGameState(GameState.GrassSkill);
            //        break;

            //    case ElementType.Dark:
            //        break;

            //    case ElementType.Light:
            //        break;

            //    case ElementType.Lightning:
            //        break;
            //}

            GM.SetGameState(GameState.SKill);

            // 스킬 효과 실행
            StartCoroutine(SkillStart(GM.ElementType));
        }
    }

    private IEnumerator SkillStart(ElementType type)
    {
        yield return new WaitForSeconds(1.5f);

        var blocks = GM.Board.blocks;
        var delBlocks = GM.Board.delBlocks;
        var downBlocks = GM.Board.downBlocks;

        var pool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
        var size = (GM.BoardSize.x * GM.BoardSize.y);

        // 스킬 이펙트 발동
        skillEffect.Play();

        switch (type)
        {
            case ElementType.Fire:
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

                // 블럭 파괴 파티클 실행
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    delBlocks[i].BlockParticle();
                }

                yield return new WaitForSeconds(.4f);

                // 블럭 파괴
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    pool.ReturnPoolableObject(delBlocks[i]);
                    blocks.Remove(delBlocks[i]);

                    // 점수 업데이트
                    DataManager.Instance.SetScore(delBlocks[i].BlockScore);
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

                yield return new WaitForSeconds(.5f);

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

                // 생성된 블럭중에 매칭되는 블럭이 있다면 파괴
                GM.Board.BlockUpdate();

                if (GM.Board.BlockCheck())
                {
                    GM.isStart = true;
                }

                // 불 스킬 패널티
                // 5초간 블럭 이동 금지
                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText("스킬 부작용으로 5초간 블럭 이동 금지!");

                /// Test
                if (GM.GameState == GameState.Play)
                {
                    GM.SetGameState(GameState.SKill);
                }

                yield return new WaitForSeconds(5f);

                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText();
                break;

            case ElementType.Ice:

                // 블럭 찾기
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].row == 0 || blocks[i].row == 1)
                    {
                        delBlocks.Add(blocks[i]);
                    }
                }

                // 파티클 실행
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    delBlocks[i].BlockParticle();
                }

                yield return new WaitForSeconds(.4f);

                // 블럭 삭제
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    pool.ReturnPoolableObject(delBlocks[i]);
                    blocks.Remove(delBlocks[i]);

                    // 점수 업데이트
                    DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                }

                yield return new WaitForSeconds(.4f);

                // 내릴 블럭 찾기
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].row > 1)
                    {
                        downBlocks.Add(blocks[i]);
                    }
                }

                // 블럭 내리기
                for (int i = 0; i < downBlocks.Count; i++)
                {
                    var targetRow = (downBlocks[i].row -= 2);

                    if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                    {
                        Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                        downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                    }
                }

                // 빈자리에 블럭 생성하기
                emptyCount = size - blocks.Count;
                colValue = 0;
                rowValue = downBlocks[downBlocks.Count - 1].row + 1;

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

                        if (colValue > 6)
                        {
                            colValue = 0;
                            rowValue++;
                        }
                    }
                }

                delBlocks.Clear();
                downBlocks.Clear();

                // 생성된 블럭중에 매칭되는 블럭이 있다면 파괴
                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText("스킬 효과로 시간 정지!");
                UIWindowManager.Instance.GetWindow<UITime>().timeStop = true;

                GM.Board.BlockUpdate();

                if (GM.Board.BlockCheck())
                {
                    GM.isStart = true;
                }
                else
                {
                    GM.SetGameState(GameState.Play);
                }

                // 10초정도 시간을 멈춘다
                yield return new WaitForSeconds(10f);

                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText();
                UIWindowManager.Instance.GetWindow<UITime>().timeStop = false;
                break;

            case ElementType.Grass:

                // 블럭 찾기
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].row == 0 || blocks[i].row == 1)
                    {
                        delBlocks.Add(blocks[i]);
                    }
                }

                // 파티클 실행
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    delBlocks[i].BlockParticle();
                }

                yield return new WaitForSeconds(.4f);

                // 블럭 삭제
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    pool.ReturnPoolableObject(delBlocks[i]);
                    blocks.Remove(delBlocks[i]);

                    // 점수 업데이트
                    DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                }

                yield return new WaitForSeconds(.4f);

                // 내릴 블럭 찾기
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].row > 1)
                    {
                        downBlocks.Add(blocks[i]);
                    }
                }

                // 블럭 내리기
                for (int i = 0; i < downBlocks.Count; i++)
                {
                    var targetRow = (downBlocks[i].row -= 2);

                    if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                    {
                        Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                        downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                    }
                }

                // 빈자리에 블럭 생성하기
                emptyCount = size - blocks.Count;
                colValue = 0;
                rowValue = downBlocks[downBlocks.Count - 1].row + 1;

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

                        if (colValue > 6)
                        {
                            colValue = 0;
                            rowValue++;
                        }
                    }
                }

                delBlocks.Clear();
                downBlocks.Clear();

                // 생성된 블럭중에 매칭되는 블럭이 있다면 파괴
                GM.Board.BlockUpdate();

                if (GM.Board.BlockCheck())
                {
                    GM.isStart = true;
                }
                else
                {
                    GM.SetGameState(GameState.Play);
                }

                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText("스킬 효과로 시간 정지 및 스킬 게이지 회복!");

                UIWindowManager.Instance.GetWindow<UITime>().timeStop = true;

                for (int i = 0; i < 6; i++)
                {
                    yield return new WaitForSeconds(1f);
                    SetSkillAmount(.05f);
                }

                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText();
                UIWindowManager.Instance.GetWindow<UITime>().timeStop = false;
                break;
        }

        yield return new WaitForSeconds(.5f);

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