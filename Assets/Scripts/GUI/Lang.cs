using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;
using System.IO;

public class Lang: MonoBehaviour
{
	/***
	 * Description:
	 * Idioma class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/
	
	/*
The Lang Class adds easy to use multiple language support to any Unity project by parsing an XML file
containing numerous strings translated into any languages of your choice.  Refer to UMLS_Help.html and lang.xml
for more information.
*/
	
	private Hashtable Strings;
	/*
	Initialize Lang class
	path = path to XML resource example:  Path.Combine(Application.dataPath, "lang.xml")
	language = language to use example:  "English"
	web = bool  indicating if resource is local or on-line example:  true if on-line, false if local
	
	NOTE:
	If XML resource is on-line rather than local do not supply the path to the path variable as stated above
	instead use the WWW class to download the resource and then supply the resource.text to this initializer
	
	Web Example:
	WWW wwwXML = new WWW("http://www.exampleURL.com/lang.xml");
	yield return wwwXML;
		
	Lang LangClass = new Lang(wwwXML.text, currentLang, true)
	*/
	public  Lang (  string path ,   string language ,   bool web   ){
		if (!web) {
			setLanguage(path, language);
		} else {
			setLanguageWeb(path, language);
		}
	}
	
	/*
	Use the setLanguage function to swap languages after the Lang class has been initialized.
	This function is called automatically when the Lang class is initialized.
	path = path to XML resource example:  Path.Combine(Application.dataPath, "lang.xml")
	language = language to use example:  "English"
	
	NOTE:
	If the XML resource is stored on the web rather than on the local system use the
	setLanguageWeb function
	*/
	public void  setLanguage (  string path ,   string language  ){
		
		XmlDocument xml = new XmlDocument();
		xml.Load(path);
		//XmlDocument xml = (XmlDocument)Resources.Load(path, typeof(XmlDocument));

		Strings = new Hashtable();
		foreach(XmlElement xmlItem in xml.SelectNodes("languages/"+language+"/string"))
		{
			Strings.Add(xmlItem.GetAttribute("name"), xmlItem.InnerText);
		}

	}
	
	/*
	Use the setLanguageWeb function to swap languages after the Lang class has been initialized
	and the XML resource is stored on the web rather than locally.  This function is called automatically
	when the Lang class is initialized.
	xmlText = string containing all XML nodes
	language = language to use example:  "English"
	
	Example:
	WWW wwwXML = new WWW("http://www.exampleURL.com/lang.xml");
	yield return wwwXML;
		
	Lang LangClass = new Lang(wwwXML.text, currentLang)
	*/
	public void  setLanguageWeb (  string xmlText ,   string language  ){
		XmlDocument xml = new XmlDocument();
		xml.Load(new StringReader(xmlText));
		
		Strings = new Hashtable();
		/*XmlElement element = xml.DocumentElement.SelectSingleNode(language);
		if (element) {
			IEnumerator elemEnum = element.GetEnumerator();
			while (elemEnum.MoveNext()) {
				XmlElement xmlItem = elemEnum.Current;
				Strings.Add(xmlItem.GetAttribute("name"), xmlItem.InnerText);
			}
		} else {
			Debug.LogError("The specified language does not exist: " + language);
		}*/
		foreach(XmlElement xmlItem in xml.SelectNodes("languages/"+language))
		{
			string nombre = xmlItem.GetAttribute("name").ToString();
			string texto = xmlItem.InnerText;
			Strings.Add(nombre, texto);
		}
	}
	
	/*
	Access strings in the currently selected language by supplying this getString function with
	the name identifier for the string used in the XML resource.
	
	Example:
	XML file:
	<languages>
		<English>
			<string name="app_name">Unity Multiple Language Support</string>
			<string name="description">This script provides convenient multiple language support.</string>
		</English>
		<French>
			<string name="app_name">Unité Langue Soutien Multiple</string>
			<string name="description">Ce script fournit un soutien multilingue pratique.</string>
		</French>
	</languages>
	
	JavaScript:
	string appName = langClass.getString("app_name");
	*/
	public string getString ( string name  ){
		if (!Strings.ContainsKey(name)) {
			Debug.LogError("The specified string does not exist: " + name);
			
			return "";
		}
		
		return Strings[name].ToString();
	}
	
	
}