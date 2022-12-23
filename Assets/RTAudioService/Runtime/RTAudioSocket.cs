using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using WebSocketSharp;

namespace Hsinpa.RTAudioService
{
    public class RTAudioSocket
    {
        WebSocket m_websocket;

        System.Action<byte[]> OnBinaryMessage;
        System.Action<string, string> OnStringMessage;

        public RTAudioSocket() {
            //this.m_url = url;

            //(var ws = new WebSocket(url))
            //{
            //    ws.OnMessage += (sender, e) =>
            //    {
            //        Debug.Log("Laputa says: " + e.Data);
            //    };

            //    ws.Connect();
            //    ws.Send("BALUS");
            //}
        }

        public void Connect(string url) {
            this.m_websocket = new WebSocket(url);
            this.m_websocket.Connect();
            this.m_websocket.OnMessage += OnMessage;
        }

        public void SendMessage(string json_string) {
            if (this.m_websocket != null && this.m_websocket.ReadyState == WebSocketState.Open)
                this.m_websocket.Send(json_string);
        }

        public void OnMessage(object sender, MessageEventArgs args) {

            if (args.IsBinary && OnBinaryMessage != null) {
                OnBinaryMessage( args.RawData );
            }

            if (OnStringMessage != null) {
                var jsonObject = JObject.Parse(args.Data);
                string event_id = jsonObject.Value<string>(RTAudioStatic.JSON.EventID);
                OnStringMessage(event_id, args.Data);
            }
        }

        ~RTAudioSocket() {
            if (this.m_websocket != null)
                this.m_websocket.Close();

            this.m_websocket = null;
        }

    }
}