import * as signalR from "@microsoft/signalr";
import { env } from "../env";

type EventCallback<T> = (data: T) => void;

export class SignalRClient {
    private connection: signalR.HubConnection;
    private connected: boolean = false;
    private eventMap: Map<string, EventCallback<any>[]> = new Map();

    constructor(hubPath: string) {
        const hubUrl = `${env.BACKEND_URL.replace(/\/$/, "")}/${hubPath.replace(/^\//, "")}`;
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .withAutomaticReconnect()
            .build();

        this.connection.onreconnected(() => {
            console.log("SignalR reconnected");
            this.connected = true;
        });

        this.connection.onclose(() => {
            console.log("SignalR disconnected");
            this.connected = false;
        });
    }

    public async start(): Promise<void> {
        if (!this.connected) {
            try {
                await this.connection.start();
                this.connected = true;
                console.log("SignalR connected to", this.connection.baseUrl);
            } catch (err) {
                setTimeout(() => this.start(), 2000);
            }
        }
    }

    public async stop(): Promise<void> {
        await this.connection.stop();
        this.connected = false;
    }

    public async invoke<T = any>(method: string, ...args: any[]): Promise<T | null> {
        if (!this.connected) await this.start();
        try {
            return await this.connection.invoke<T>(method, ...args);
        } catch (err) {
            return null;
        }
    }

    public on<T = any>(event: string, callback: EventCallback<T>): void {
        if (!this.eventMap.has(event)) {
            this.eventMap.set(event, []);
            this.connection.on(event, (data: T) => {
                this.eventMap.get(event)?.forEach(cb => cb(data));
            });
        }
        this.eventMap.get(event)?.push(callback);
    }

    public off<T = any>(event: string, callback?: EventCallback<T>): void {
        if (!callback) {
            this.eventMap.delete(event);
            this.connection.off(event);
        } else {
            const callbacks = this.eventMap.get(event);
            if (!callbacks) return;
            this.eventMap.set(event, callbacks.filter(cb => cb !== callback));
        }
    }
}
