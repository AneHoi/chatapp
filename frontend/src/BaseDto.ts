export class BaseDto<T> {
  eventType: string;

  constructor(init?: Partial<T>) {
    this.eventType = this.constructor.name;
    Object.assign(this, init)
  }
}

export class ServerEchosClientDto extends BaseDto<ServerEchosClientDto>{
  message?: string;
}
export class ServerSendsMessageBroadDto extends BaseDto<ServerSendsMessageBroadDto>{
  message?: string;
}
