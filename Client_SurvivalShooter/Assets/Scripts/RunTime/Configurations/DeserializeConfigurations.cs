public static class DeserializeConfigurations 
{
	public static void DeserilaizeConfigs()
	{
		CharacterCfg.Deserialize();
		ItemCfg.Deserialize();
		PlayerInputCfg.Deserialize();
		WeaponCfg.Deserialize();
	}
}