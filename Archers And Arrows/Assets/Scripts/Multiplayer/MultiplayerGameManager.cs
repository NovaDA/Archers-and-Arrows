using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;

namespace RhinoGame
{
    public class MultiplayerGameManager : MonoBehaviourPunCallbacks
    {
        public static MultiplayerGameManager Instance = null;
        public int MaxScore = 5;
        public Text InfoText;

        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            StartGame();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MultiplayerLobby");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                //PhotonNetwork.Disconnect();
                CheckEndOfGame();
            }
        }

        private void StartGame()   
        {
            PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            //if (!PhotonNetwork.IsMasterClient)
            //{
            //    return;
            //}
            if (changedProps.ContainsKey("score"))
            {
                CheckEndOfGame();
            }
        }

        private void CheckEndOfGame()
        {
            bool showGameOver = false;

            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                Debug.Log("Player: " + p.NickName + "Score: " + p.GetScore());
                
                if (p.GetScore() >= MaxScore || PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    showGameOver = true;
                    break;
                }
            }

            Debug.Log("====================================");

            Debug.Log("showGameOver: " + showGameOver);

            if (showGameOver)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StopAllCoroutines();
                }

                string winner = "";
                int score = -1;
                Color color = Color.black;

                foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                {
                    if (p.GetScore() > score)
                    {
                        winner = p.NickName;
                        score = p.GetScore();
                        color = AsteroidsGame.GetColor(p.GetPlayerNumber());
                    }
                }

                StartCoroutine(EndOfGame(winner, score, color));
            }
        }

        private IEnumerator EndOfGame(string winner, int score, Color color)
        {
            Debug.Log("EndOfGame!!!");
            Debug.Log("winner: " + winner);
            Debug.Log("score: " + score);
            float timer = 5.0f;

            while (timer > 0.0f)
            {
                InfoText.color = color;
                InfoText.text = string.Format("Player {0} won with {1} points.\n\n\nReturning to login screen in {2} seconds.", winner, score, timer.ToString("n2"));

                yield return new WaitForEndOfFrame();

                timer -= Time.deltaTime;
            }

            PhotonNetwork.LeaveRoom();
        }

    }
}