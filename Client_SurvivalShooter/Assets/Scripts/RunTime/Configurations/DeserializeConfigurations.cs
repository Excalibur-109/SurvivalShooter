public static class DeserializeConfigurations 
{
	public static void DeserilaizeConfigs()
	{
		BulletCfg.Deserialize();
		CharacterCfg.Deserialize();
		ItemCfg.Deserialize();
		PlayerInputCfg.Deserialize();
		SpawnCfg.Deserialize();
		WeaponCfg.Deserialize();
	}
}