using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class Get : MonoBehaviour
{

    public RawImage SprPokemon;
    public Text NombrePokemon;
    public Text NumPokemon;
    public Text[] TipoPokemon;


    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    void Start()
    {
        // A correct website page.
        //StartCoroutine(GetRequest("https://www.example.com"));

        // A non-existing page.
        //StartCoroutine(GetRequest("https://error.html"));
    }

    public void GetRandomPokemon()
    {
        int randomPokeIndex = Random.Range(1, 808);

        SprPokemon.texture = Texture2D.blackTexture;

        NombrePokemon.text = "Cargando...";

        NumPokemon.text = "#" + randomPokeIndex.ToString();

        foreach (Text tipo in TipoPokemon)
        {
            tipo.text = "";
        }

        StartCoroutine(GetPokemon(randomPokeIndex));

    }

    [System.Obsolete]
    IEnumerator GetPokemon(int numPokemon)
    {

        string pokemonURL = basePokeURL + "pokemon/" + numPokemon.ToString();

        //Get info

        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);

        yield return pokeInfoRequest.SendWebRequest();

        if(pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        string pokeName = pokeInfo["name"];
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        JSONNode pokeTypes = pokeInfo["types"];
        string[] pokeTypeNames = new string[pokeTypes.Count];

        for(int i = 0, j = pokeTypes.Count - 1; i < pokeTypes.Count; i++, j--)
        {
            pokeTypeNames[j] = pokeTypes[i]["type"]["name"];
        }

        //Get sprite

        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);

        yield return pokeSpriteRequest.SendWebRequest();

        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }

        //Set UI Objects
        SprPokemon.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        SprPokemon.texture.filterMode = FilterMode.Point;

        //NombrePokemon.text = pokeName;
        NombrePokemon.text = CapitalizeFirstLetter(pokeName);

        for (int i = 0; i < TipoPokemon.Length; i++)
        {
            try
            {
                //TipoPokemon[i].text = pokeTypeNames[i];
                TipoPokemon[i].text = CapitalizeFirstLetter(pokeTypeNames[i]);
            }
            catch (IndexOutOfRangeException)
            {
                TipoPokemon[i].text = "";
            }

        }

    }

    private string CapitalizeFirstLetter(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }

    /*IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            //if (UnityWebRequest.result == UnityWebRequest.Result.ConnectionError)
            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }*/

    /*IEnumerator GetText()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://www.my-server.com"))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;
            }
        }
    }*/

}
