﻿using System.Net;

namespace ET.Server
{
    [Invoke((long)SceneType.Match)]
    public class FiberInit_Match: AInvokeHandler<FiberInit, ETTask>
    {
        public override async ETTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            root.AddComponent<ActorInnerComponent>();
            root.AddComponent<ActorSenderComponent>();
            root.AddComponent<MatchComponent>();
            root.AddComponent<LocationProxyComponent>();
            root.AddComponent<ActorLocationSenderComponent>();

            await ETTask.CompletedTask;
        }
    }
}