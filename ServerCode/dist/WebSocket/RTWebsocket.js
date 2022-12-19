"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const ws_1 = require("ws");
const uuid_1 = require("uuid");
const RTWebsocketUser_1 = require("./RTWebsocketUser");
class RTWebsocket {
    constructor(port) {
        this.m_wss = new ws_1.WebSocketServer({ port: port });
        this.m_user_container = new RTWebsocketUser_1.default();
        this.RegisterBasicEvent();
    }
    RegisterBasicEvent() {
        let self = this;
        this.m_wss.on('connection', function connection(ws, req) {
            ws.id = (0, uuid_1.v4)();
            self.m_user_container.AddUser(ws);
            console.log('Connect ' + ws.id);
            ws.on('message', function message(data, isBinary) {
                console.log('received: %s', data);
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
    BroadcastAudioData(data, socket_id) {
        let candidates = this.m_user_container.FilterAvailableListener(socket_id);
        let lens = candidates.length;
        for (let i = 0; i < lens; i++) {
            candidates[i].send(data, { binary: true });
        }
    }
}
exports.default = RTWebsocket;
//# sourceMappingURL=RTWebsocket.js.map