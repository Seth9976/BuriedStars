using System;
using System.Collections.Generic;
using System.Xml;
using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Convert an csv string into a Xml node")]
public class ConvertCsvStringToXmlNode : DataMakerXmlNodeActions
{
	[Tooltip("The Csv string")]
	[RequiredField]
	public FsmString csvSource;

	[Tooltip("If the csv first line is a headerm check this, it will allow you to use keys to access columns instead of indexes")]
	public FsmBool hasHeader;

	[ActionSection("Result")]
	[Tooltip("Save as xml reference")]
	public FsmString storeReference;

	[Tooltip("Save in DataMaker Xml Proxy component")]
	[CheckForComponent(typeof(DataMakerXmlProxy))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Author defined Reference of the DataMaker Xml Proxy component ( necessary if several component coexists on the same GameObject")]
	public FsmString reference;

	[Tooltip("Save as string")]
	public FsmString xmlString;

	public FsmEvent errorEvent;

	private string[] _csvHeader;

	public override void Reset()
	{
		csvSource = null;
		hasHeader = null;
		storeReference = new FsmString
		{
			UseVariable = true
		};
		gameObject = null;
		reference = new FsmString
		{
			UseVariable = true
		};
		xmlString = new FsmString
		{
			UseVariable = true
		};
	}

	public override void OnEnter()
	{
		ConvertFromCsvString();
		Finish();
	}

	private void ConvertFromCsvString()
	{
		CsvData csvData = CsvReader.LoadFromString(csvSource.Value, hasHeader.Value);
		XmlDocument xmlDocument = new XmlDocument();
		XmlNode xmlNode = xmlDocument.AppendChild(xmlDocument.CreateElement("Root"));
		try
		{
			if (csvData.HasHeader)
			{
				_csvHeader = csvData.HeaderKeys.ToArray();
			}
			foreach (List<string> datum in csvData.Data)
			{
				XmlNode xmlNode2 = xmlDocument.CreateElement("Record");
				for (int i = 0; i < datum.Count; i++)
				{
					XmlNode xmlNode3 = xmlDocument.CreateElement((!csvData.HasHeader) ? "Field" : _csvHeader[i]);
					xmlNode3.InnerText = datum[i];
					xmlNode2.AppendChild(xmlNode3);
				}
				xmlNode.AppendChild(xmlNode2);
			}
		}
		catch (Exception ex)
		{
			Fsm.EventData.StringData = ex.Message;
			base.Fsm.Event(errorEvent);
			return;
		}
		if (xmlDocument == null)
		{
			Fsm.EventData.StringData = DataMakerXmlUtils.lastError;
			base.Fsm.Event(errorEvent);
			return;
		}
		if (!storeReference.IsNone && xmlDocument.DocumentElement.GetType() != typeof(XmlNodeList))
		{
			DataMakerXmlUtils.XmlStoreNode(xmlDocument.DocumentElement, storeReference.Value);
		}
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		DataMakerXmlProxy dataMakerXmlProxy = DataMakerCore.GetDataMakerProxyPointer(typeof(DataMakerXmlProxy), ownerDefaultTarget, reference.Value, silent: false) as DataMakerXmlProxy;
		if (dataMakerXmlProxy != null && xmlDocument.DocumentElement.GetType() != typeof(XmlNodeList))
		{
			dataMakerXmlProxy.InjectXmlNode(xmlDocument.DocumentElement);
		}
		dataMakerXmlProxy.RefreshStringVersion();
		if (!xmlString.IsNone)
		{
			xmlString.Value = dataMakerXmlProxy.content;
		}
		Finish();
	}
}
