import { WebSocketServer, WebSocket } from 'ws';
import {Dictionary} from 'typescript-collections';
import {User} from './RTWebsocketStruct';
import { v4 as uuidv4 }  from 'uuid';
import {AudioWebsocket} from './RTWebsocketStruct';

export default class RTWwebsocketUser {

    m_socket_dict : Dictionary<string, User>;
    m_user_dict : Dictionary<string, User>;

    constructor() {
        this.m_socket_dict = new Dictionary();
    }

    public AddUser(websocket: AudioWebsocket) {
        let new_user : User = {id: "", channel : null, register:[], socket : websocket };
        this.m_socket_dict.setValue(websocket.id, new_user);
    }

    public RemoveUser(socket_id : string) {
        if (!this.m_socket_dict.containsKey(socket_id)) return;

        let user = this.m_socket_dict.getValue(socket_id);

        this.m_user_dict.remove(user.id);
        this.m_socket_dict.remove(socket_id);
    }

    public SetUser(user_id: string, websocket: AudioWebsocket) {

        if (!this.m_socket_dict.containsKey(websocket.id)) return;

        let user = this.m_socket_dict.getValue(websocket.id);
        
        user.id = user_id;

        this.m_user_dict.setValue(user_id, user);
    }

    //Subscribe to user id sets
    public SubscribeUser(subscribe_set: string[], self_user_id: string) {
        if (subscribe_set == null) return;

        for (let s_id in subscribe_set) {

            if (!this.m_user_dict.containsKey(s_id)) continue;

            let user = this.m_user_dict.getValue(s_id);
    
            user.register.push(self_user_id)    
        }
    }

    //Unsubscribe to listen from
    public UnSubscribeUser(unsubscribe_set: string[], self_user_id: string) {
        if (unsubscribe_set == null) return;
        
        let setLens = unsubscribe_set.length;

        for (let i = setLens - 1; i >= 0; i--) {
            if (!this.m_user_dict.containsKey(unsubscribe_set[i])) continue;

            let user = this.m_user_dict.getValue(unsubscribe_set[i]);
    
            let remove_index = user.register.findIndex(x=> x == self_user_id);

            if (remove_index >= 0)
                user.register.splice(remove_index, 1);
        }
    }

    public FilterAvailableListener(socket_id: string) {
        let user = this.m_socket_dict.getValue(socket_id);

        if (user == null) return [];

        let socketArray: WebSocket[] = [];

        for (let register_id in user.register) {
            let register = user = this.m_user_dict.getValue(register_id);

            if (register != null && register.socket.readyState === WebSocket.OPEN)
                socketArray.push(register.socket);
        }

        return socketArray;
    }

};