using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;


public class DialogeTree
{
	private class DialogeResponse
	{
		public string text;
		public string result;
	}

	private class DialogeEvent
	{
		public string title;
		public string speaker;
		public string speech;
		public List<DialogeResponse> responses;
		public string result = "";

		public List<string> GetResponseOptions()
		{
			List<string> res = new List<string>();
			if (this.result == "end")
			{
				return res;
			}
			foreach (DialogeResponse response in responses)
			{
				res.Add(response.text);
			}
			return res;
		}

		public string GetOptionResult(int option)
		{
			return this.responses[option].result;
		}
	}

	private List<DialogeEvent> events;
	private int event_p;
	private bool isOngoing = true;
	private DialogeEvent currentEvent;

	public bool valid;

	public DialogeTree(string JSONPath)
	{
		using (StreamReader r = new StreamReader(JSONPath))
		{
			string json = r.ReadToEnd();
			this.events = JsonConvert.DeserializeObject<List<DialogeEvent>>(json);
		}
		this.currentEvent = this.events[0];
		this.valid = this.ValidateTree();
		if (!this.valid)
		{
			Debug.Log("tree could not be validated");
		}
	}

	public string GetSpeaker()
	{
		return this.currentEvent.speaker;
	}

	public string GetSpeech()
	{
		return this.currentEvent.speech;
	}

	public bool IsOngoing()
	{
		return this.isOngoing;
	}

	public bool IsLastEvent()
	{
		Debug.Log(this.currentEvent.result);
		return this.currentEvent.result == "end";
	}

	public List<string> GetResponseOptions()
	{
		return this.currentEvent.GetResponseOptions();
	}

	public bool SelectOption(int option)
	{
		string result = this.currentEvent.GetOptionResult(option);
		Debug.Log(result);
		if (result == "end")
		{
			Debug.Log("Setting ongoing to false");
			this.isOngoing = false;
			return true;
		}
		for (int i = 0; i < this.events.Count; i++)
		{
			if (this.events[i].title == result)
			{
				this.currentEvent = this.events[i];
				return true;
			}
		}
		Debug.Log("Attempted to find a nonexistent event");
		return false;
	}

	// the goal is to catch mistakes in the JSON setup early so that
	// we don't have a hellish time debugging
	private bool ValidateTree()
	{
		List<string> titles = new List<string>();
		foreach (DialogeEvent dialogEvent in this.events)
		{
			titles.Add(dialogEvent.title);
		}
		foreach (DialogeEvent dialogEvent in this.events)
		{
			//events are allowed to have no responses so long as they have a result
			if (dialogEvent.responses == null && dialogEvent.result == null)
			{
				Debug.Log("Event " + dialogEvent.title + " has no responses or results!");
				return false;
			}
			else if (dialogEvent.responses == null && dialogEvent.result != null)
			{
				continue;
			}

			if (dialogEvent.responses.Count > 4)
			{
				Debug.Log("Event " + dialogEvent.title + " has more than 4 response opts");
				return false;
			}


			foreach (DialogeResponse response in dialogEvent.responses)
			{
				if (response.result == "end" || titles.Contains(response.result))
				{
					continue;
				}
				else
				{
					Debug.Log("Event " + response.result + " does not exist.");
					return false;
				}
			}
		}
		return true;
	}
}
