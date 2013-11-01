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
		string caminho = "file:///"+Application.persistentDataPath+"/"+fileName;
		
		WWW reader = new WWW (caminho);
		yield return reader;
		
		if (reader.error != null)
		{
			//Debug.Log ("Erro lendo arquivo xml: "+reader.error);
			
			tilesetXML = Resources.Load("Tileset") as TextAsset;
			rawXML = tilesetXML.text;
			readFromXML();
		}
		else
		{
			//Debug.Log("loaded DOCUMENTS xml");
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
