using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class LogoPanel : MonoBehaviour 
{
	public UIInteractivePanel logoPanel;
	
	void Start ()
	{
		if(Save.HasKey(PlayerPrefsKeys.PLAYEREXPERIENCE)) Flow.playerExperience = Save.GetFloat(PlayerPrefsKeys.PLAYEREXPERIENCE);
		if(Save.HasKey(PlayerPrefsKeys.PLAYERLEVEL)) Flow.playerLevel = Save.GetInt(PlayerPrefsKeys.PLAYERLEVEL);
		if(Save.HasKey(PlayerPrefsKeys.HPLEVEL)) Flow.hpLevel = Save.GetInt(PlayerPrefsKeys.HPLEVEL);
		if(Save.HasKey(PlayerPrefsKeys.MAPLEVEL)) Flow.mapLevel = Save.GetInt(PlayerPrefsKeys.MAPLEVEL);
		if(Save.HasKey(PlayerPrefsKeys.RADARLEVEL)) Flow.radarLevel = Save.GetInt(PlayerPrefsKeys.RADARLEVEL);
		
		/*Flow.playerExperience = 0;
		Flow.playerLevel = 13;
		Flow.hpLevel = 5;
		Flow.mapLevel = 5;
		Flow.radarLevel = 5;*/
		
		//Debug.Log("peguei level " + Flow.playerLevel + " na key " + PlayerPrefsKeys.PLAYERLEVEL);
		
		StartCoroutine(getXML());
		
		logoPanel.AddTempTransitionDelegate(delegate
		{
			Application.LoadLevel("Mainmenu");	
		});
	}
	
	string fileName = "Tileset.xml";
	TextAsset tilesetXML;
	string rawXML;
	
	IEnumerator getXML()
	{	
		Flow.header.levelText.Text = "Level " + Flow.playerLevel.ToString();
		Flow.header.experienceText.Text = "Exp " + Flow.playerExperience.ToString();
		Flow.header.expBar.width = 7 * Flow.playerExperience/(Flow.playerLevel * Flow.playerLevel * 100);
		Flow.header.expBar.CalcSize();
		
		string caminho = "file:///"+Application.persistentDataPath+"/"+fileName;
		Debug.Log(caminho);
		
		WWW reader = new WWW (caminho);
		yield return reader;
		
		if (reader.error != null)
		{
			Debug.Log ("Erro lendo arquivo xml: "+reader.error);
			
			tilesetXML = Resources.Load("Tileset") as TextAsset;
			rawXML = tilesetXML.text;
			readFromXML();
		}
		else
		{
			Debug.Log("loaded DOCUMENTS xml");
			
			rawXML = reader.text;
			readFromXML();
		}
	}
	
	void readFromXML()
	{
		XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(rawXML);
		
		foreach(XmlNode masterNode in xmlDoc.SelectNodes("Document/Worlds/World"))
		{
			World tempWorld = new World();
			tempWorld.levelDict = new Dictionary<int, Level>();
			tempWorld.id = masterNode.Attributes["ID"].InnerText.ToInt32();
			
			foreach(XmlNode subNode in masterNode.SelectNodes("Levels/Level"))
			{
				Level tempLevel = new Level();
				tempLevel.tileset = new List<int>();
				tempLevel.id = subNode.Attributes["ID"].InnerText.ToInt32();
				tempLevel.lastUpdate = DateTime.Parse(subNode.SelectSingleNode("Update").InnerText);
				
				string s = subNode.SelectSingleNode("Tileset").InnerText;
				for(int i = 0; i< s.Length; i++)
				{
					tempLevel.tileset.Add(int.Parse(s[i].ToString()));
				}
				tempWorld.levelDict.Add(tempLevel.id, tempLevel);
			}
			
			Flow.worldDict.Add(tempWorld.id, tempWorld);
		}
		
		foreach(XmlNode customStage in xmlDoc.SelectNodes("Document/CustomStages/CustomStage"))
		{
			List<List<int>> customTileset = new List<List<int>>();
			string all64 = customStage.SelectSingleNode("Tileset").InnerText;
			int customMines = 0;
			for(int i = 0; i< 8; i++)
			{
				List<int> singleRow = new List<int>();
				for(int j = 0; j < 8; j++)
				{
					if(int.Parse(all64[i*8+j].ToString()) == 1) customMines++;
					singleRow.Add(int.Parse(all64[i*8+j].ToString()));
				}
				customTileset.Add(singleRow);
			}
			
			if(Flow.customStages == null)
			{
				Flow.customStages = new List<CustomStage>();
			}
			CustomStage c = new CustomStage();
			c.tileset = customTileset;
			c.world = int.Parse(customStage.SelectSingleNode("World").InnerText);
			c.numberOfMines = customMines;
			c.name = customStage.Attributes["Name"].InnerText;
			c.id = int.Parse(customStage.SelectSingleNode("ID").InnerText);
			Flow.customStages.Add(c);
			
			//Debug.Log("adicionei o level " + customStage.Attributes["Name"].InnerText);
			//Debug.Log("o tileset dele era " + all64);
		}
		
		/*foreach(CustomStage c in Flow.customStages)
		{
			string testTileset = "";
			foreach(List<int> listInt in c.tileset)
			{
				foreach(int i in listInt) testTileset += i;
			}
			Debug.Log(testTileset);
		}*/
		
		logoPanel.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
	}
	
	public void WriteToXML()
	{
		string filename = Application.persistentDataPath+"/"+fileName;
		using (FileStream fileStream = new FileStream(filename, FileMode.Create))
		using (StreamWriter sw = new StreamWriter(fileStream))
		using (XmlTextWriter writer = new XmlTextWriter(sw))
		{
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 4;
			
			writer.WriteStartDocument();
			writer.WriteStartElement("Document");
		    writer.WriteStartElement("Worlds");
			
		    for(int i = 0 ; i < 5 ; i++)
		    {
				writer.WriteStartElement("World");
				writer.WriteAttributeString("ID", i.ToString());
				writer.WriteStartElement("Levels");
				for(int j = 0; j < 9; j++)
				{
					writer.WriteStartElement("Level");
					writer.WriteAttributeString("ID", j.ToString());
					writer.WriteElementString("Update", DateTime.UtcNow.ToString());
					
					string tempString = "";
					foreach(int k in Flow.worldDict[i].levelDict[j].tileset) tempString += k.ToString();
					
					writer.WriteElementString("Tileset", tempString);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
				writer.WriteEndElement();
		    }
		    writer.WriteEndElement();
			writer.WriteEndElement();
		    writer.WriteEndDocument();
		}
	}
}
