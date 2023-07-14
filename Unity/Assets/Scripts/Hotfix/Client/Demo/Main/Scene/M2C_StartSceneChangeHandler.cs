﻿namespace ET.Client
{
	[ActorMessageHandler(SceneType.Demo)]
	public class M2C_StartSceneChangeHandler : ActorMessageHandler<Scene, M2C_StartSceneChange>
	{
		protected override async ETTask Run(Scene root, M2C_StartSceneChange message)
		{
			await SceneChangeHelper.SceneChangeTo(root, message.SceneName, message.SceneInstanceId);
		}
	}
}
