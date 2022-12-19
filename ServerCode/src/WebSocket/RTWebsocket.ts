import { WebSocketServer, WebSocket, RawData } from 'ws';
import { v4 as uuidv4 }  from 'uuid';
import {AudioWebsocket} from './RTWebsocketStruct';
import RTWwebsocketUser from './RTWebsocketUser';

export default class RTWebsocket {
    m_wss : WebSocketServer;
    m_user_container: RTWwebsocketUser;

    constructor(port: number) {
        this.m_wss =  new WebSocketServer({ port: port });
        this.m_user_container = new RTWwebsocketUser();
        this.RegisterBasicEvent();
    }
    
    RegisterBasicEvent() {
        let self = this;

        this.m_wss.on('connection', function connection(ws : AudioWebsocket, req) {
            ws.id = uuidv4();

            self.m_user_container.AddUser(ws);

            console.log('Connect ' + ws.id);
        
            ws.on('message', function message(data, isBinary) {
                console.log('received: %s', data);

                //Send Audio
                if (isBinary) {
                    
                }

            });

            ws.on('close', function message(data) {
                self.m_user_container.RemoveUser(ws.id);
            });

            ws.on('error', function message(data) {
                self.m_user_container.RemoveUser(ws.id);
            });
        
        });
    }

    BroadcastAudioData(data: RawData, socket_id : string) {
        let candidates = this.m_user_container.FilterAvailableListener(socket_id);
        let lens = candidates.length;

        for (let i = 0; i < lens; i++) {
            candidates[i].send(data, {binary: true});
        }
    }
}