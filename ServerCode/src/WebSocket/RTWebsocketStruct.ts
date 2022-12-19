import { WebSocket } from 'ws';

export interface AudioWebsocket extends WebSocket {id: string};

export interface User {
    id : string,
    register: string[],
    channel: string,
    socket: WebSocket,
}

