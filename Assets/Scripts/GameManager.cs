using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public Player player;
    List<Player> players;
    public int maxPlayersCount;
    public Vector2Int sizeMatrixPlayerSpawn;

    class Edge
    {
        public Vector2Int num;
        public int count;

        public Edge(Vector2Int num, int count)
        {
            this.num = num;
            this.count = count;
        }
    }

    public RenderTexture renderTexture;
    Texture2D texture;
    public Material brush;
    public Texture2D brushTexture;

    public GameObject drawBar;
    bool draw;
    bool win;

    public Material transition;


    SplineFollower splineFollower;

    void Start()
    {
        splineFollower = GetComponent<SplineFollower>();
        players = new List<Player>();

        renderTexture.Release();
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            transition.SetFloat("_Fill", Mathf.Lerp(1, 0, t));
            yield return null;
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(0.5f);
        float t = 0;
        while (t < 0.66f)
        {
            t += Time.deltaTime;
            transition.SetFloat("_Fill", Mathf.Lerp(0, -1, t / 0.66f));
            yield return null;
        }

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    IEnumerator EndWinGame()
    {
        yield return new WaitForSeconds(4f);
        float t = 0;
        while (t < 0.66f)
        {
            t += Time.deltaTime;
            transition.SetFloat("_Fill", Mathf.Lerp(0, -1, t / 0.66f));
            yield return null;
        }

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    void Update()
    {
        if (transform.position.z > 177 && !win)
        {
            win = true;
            GetComponent<AudioSource>().Play();
            for (int i = 0; i < players.Count; i++) players[i].animator.SetBool("Victory", true);
            StartCoroutine(EndWinGame());
        }
    }

    public void StartDraw()
    {
        draw = true;
    }

    public void Draw()
    {
        RenderTexture.active = renderTexture;

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, renderTexture.width, 0, renderTexture.height);
        Graphics.DrawTexture(new Rect(Input.mousePosition.x * ((float)renderTexture.width / Screen.width), Input.mousePosition.y * ((float)renderTexture.height / Screen.height), 40, 40), brushTexture, brush);
        GL.PopMatrix();

        RenderTexture.active = null;
    }

    public void EndDraw()
    {
        if (draw)
        {
            List<Edge> edgeList = new List<Edge>();
            int[,] matrix = new int[sizeMatrixPlayerSpawn.x, sizeMatrixPlayerSpawn.y];

            RenderTexture targetRenderTexture = RenderTexture.GetTemporary(sizeMatrixPlayerSpawn.x, sizeMatrixPlayerSpawn.y, 0, RenderTextureFormat.Default);
            Graphics.Blit(renderTexture, targetRenderTexture);
            Texture2D texture = new Texture2D(sizeMatrixPlayerSpawn.x, sizeMatrixPlayerSpawn.y, TextureFormat.Alpha8, false);
            RenderTexture.active = targetRenderTexture;
            texture.ReadPixels(new Rect(0, 0, sizeMatrixPlayerSpawn.x, sizeMatrixPlayerSpawn.y), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            targetRenderTexture.Release();

            for (int i = 0; i < sizeMatrixPlayerSpawn.x; i++)
                for (int j = 0; j < sizeMatrixPlayerSpawn.y; j++)
                    if (texture.GetPixel(i, j).a > 0.5f)
                    {
                        matrix[i, j]++;
                    }

            for (int i = 0; i < sizeMatrixPlayerSpawn.x; i++)
                for (int j = 0; j < sizeMatrixPlayerSpawn.y; j++)
                {
                    edgeList.Add(new Edge(new Vector2Int(i, j), matrix[i, j]));
                    Debug.Log(matrix[i, j]);
                }

            edgeList = edgeList.OrderBy(x => x.count).ToList();

            int n = edgeList.Count - 1;
            for (int i = 0; i < maxPlayersCount; i++)
            {
                CreatePlayer(new Vector3(Mathf.Lerp(-5, 5, (float)edgeList[n].num.x / sizeMatrixPlayerSpawn.x) + Random.Range(0, 0.33f), 0.365f, Mathf.Lerp(-5, 5, (float)edgeList[n].num.y / sizeMatrixPlayerSpawn.y) + Random.Range(0, 0.33f)));
                if (edgeList[n - 1].count >= edgeList[n].count) n--;
            }

            drawBar.SetActive(false);
            splineFollower.follow = true;
        }
    }

    public void CreatePlayer(Vector3 pos)
    {
        Player pl = Instantiate(player, transform);
        pl.transform.localPosition = pos;
        pl.Init(this);
        players.Add(pl);
    }

    public void CreatePlayerWorld(Vector3 pos)
    {
        Player pl = Instantiate(player, transform);
        pl.transform.position = pos;
        pl.Init(this);
        players.Add(pl);
    }

    public void RemovePlayer(Player player)
    {
        players.Remove(player);
        if (players.Count == 0)
        {
            splineFollower.follow = false;
            StartCoroutine(EndGame());
        }
    }
}
