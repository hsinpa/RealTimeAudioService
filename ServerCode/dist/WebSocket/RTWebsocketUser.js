"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const ws_1 = require("ws");
const typescript_collections_1 = require("typescript-collections");
class RTWwebsocketUser {
    constructor() {
        this.m_socket_dict = new typescript_collections_1.Dictionary();
    }
    AddUser(websocket) {
        let new_user = { id: "", channel: null, register: [], socket: websocket };
        this.m_socket_dict.setValue(websocket.id, new_user);
    }
    RemoveUser(socket_id) {
        if (!this.m_socket_dict.containsKey(socket_id))
            return;
        let user = this.m_socket_dict.getValue(socket_id);
        this.m_user_dict.remove(user.id);
        this.m_socket_dict.remove(socket_id);
    }
    SetUser(user_id, websocket) {
        if (!this.m_socket_dict.containsKey(websocket.id))
            return;
        let user = this.m_socket_dict.getValue(websocket.id);
        user.id = user_id;
        this.m_user_dict.setValue(user_id, user);
    }
    SubscribeUser(subscribe_set, self_user_id) {
        if (subscribe_set == null)
            return;
        for (let s_id in subscribe_set) {
            if (!this.m_user_dict.containsKey(s_id))
                continue;
            let user = this.m_user_dict.getValue(s_id);
            user.register.push(self_user_id);
        }
    }
    UnSubscribeUser(unsubscribe_set, self_user_id) {
        if (unsubscribe_set == null)
            return;
        let setLens = unsubscribe_set.length;
        for (let i = setLens - 1; i >= 0; i--) {
            if (!this.m_user_dict.containsKey(unsubscribe_set[i]))
                continue;
            let user = this.m_user_dict.getValue(unsubscribe_set[i]);
            let remove_index = user.register.findIndex(x => x == self_user_id);
            if (remove_index >= 0)
                user.register.splice(remove_index, 1);
        }
    }
    FilterAvailableListener(socket_id) {
        let user = this.m_socket_dict.getValue(socket_id);
        if (user == null)
            return [];
        let socketArray = [];
        for (let register_id in user.register) {
            let register = user = this.m_user_dict.getValue(register_id);
            if (register != null && register.socket.readyState === ws_1.WebSocket.OPEN)
                socketArray.push(register.socket);
        }
        return socketArray;
    }
}
exports.default = RTWwebsocketUser;
;
//# sourceMappingURL=RTWebsocketUser.js.map