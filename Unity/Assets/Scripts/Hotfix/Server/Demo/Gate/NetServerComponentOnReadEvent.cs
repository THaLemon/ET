﻿namespace ET.Server
{
    [Event(SceneType.Realm | SceneType.Gate | SceneType.BenchmarkServer)]
    public class NetServerComponentOnReadEvent: AEvent<Scene, NetServerComponentOnRead>
    {
        protected override async ETTask Run(Scene scene, NetServerComponentOnRead args)
        {
            Session session = args.Session;
            object message = args.Message;
            Scene root = scene.Root();
            if (message is IResponse response)
            {
                session.OnResponse(response);
                return;
            }
            // 根据消息接口判断是不是Actor消息，不同的接口做不同的处理,比如需要转发给Chat Scene，可以做一个IChatMessage接口
            switch (message)
            {
                case FrameMessage frameMessage:
                {
                    Player player = session.GetComponent<SessionPlayerComponent>().Player;
                    ActorId roomActorId = player.GetComponent<PlayerRoomComponent>().RoomActorId;
                    frameMessage.PlayerId = player.Id;
                    root.GetComponent<ActorSenderComponent>().Send(roomActorId, frameMessage);
                    break;
                }
                case IActorRoom actorRoom:
                {
                    Player player = session.GetComponent<SessionPlayerComponent>().Player;
                    ActorId roomActorId = player.GetComponent<PlayerRoomComponent>().RoomActorId;
                    actorRoom.PlayerId = player.Id;
                    root.GetComponent<ActorSenderComponent>().Send(roomActorId, actorRoom);
                    break;
                }
                case IActorLocationMessage actorLocationMessage:
                {
                    long unitId = session.GetComponent<SessionPlayerComponent>().Player.Id;
                    root.GetComponent<ActorLocationSenderComponent>().Get(LocationType.Unit).Send(unitId, actorLocationMessage);
                    break;
                }
                case IActorLocationRequest actorLocationRequest: // gate session收到actor rpc消息，先向actor 发送rpc请求，再将请求结果返回客户端
                {
                    long unitId = session.GetComponent<SessionPlayerComponent>().Player.Id;
                    int rpcId = actorLocationRequest.RpcId; // 这里要保存客户端的rpcId
                    long instanceId = session.InstanceId;
                    IResponse iResponse = await root.GetComponent<ActorLocationSenderComponent>().Get(LocationType.Unit).Call(unitId, actorLocationRequest);
                    iResponse.RpcId = rpcId;
                    // session可能已经断开了，所以这里需要判断
                    if (session.InstanceId == instanceId)
                    {
                        session.Send(iResponse);
                    }
                    break;
                }
                case IActorRequest actorRequest:  // 分发IActorRequest消息，目前没有用到，需要的自己添加
                {
                    break;
                }
                case IActorMessage actorMessage:  // 分发IActorMessage消息，目前没有用到，需要的自己添加
                {
                    break;
                }
				
                default:
                {
                    // 非Actor消息
                    MessageDispatcherComponent.Instance.Handle(session, message);
                    break;
                }
            }
        }
    }
}