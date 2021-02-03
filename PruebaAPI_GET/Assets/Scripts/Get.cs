/*
 * NOTA: El codigo no es mio, pertenece a  "Turbo Makes Games"
 * Video: https://www.youtube.com/watch?v=GIxu8kA9EBU
 */

//Importaciones que uso
#region Importaciones
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;
#endregion


public class Get : MonoBehaviour
{

    //Todas las variables
    #region Variables_que_uso

    //UIs para que funcione la cosa
    #region UIs
    public RawImage SprPokemon;
    public Text NombrePokemon;
    public Text NumPokemon;
    public Text[] TipoPokemon;
    #endregion

    //Enlace para la Api de Pokemon
    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    #endregion

    //Genera un numero aleatorio con todos los pokemones que existen registrados en la pagina
    public void GetRandomPokemon()
    {

        //Coge el valor aleatorio
        int randomPokeIndex = Random.Range(1, 808);

        //Vacio todos los datos de las UIs hasta que lleguen los datos del GET
        #region VaciadoUIs
        SprPokemon.texture = Texture2D.blackTexture;

        NombrePokemon.text = "Cargando...";

        NumPokemon.text = "#" + randomPokeIndex.ToString();

        foreach (Text tipo in TipoPokemon)
        {
            tipo.text = "";
        }
        #endregion

        //Cargo en hilo el Get
        StartCoroutine(GetPokemon(randomPokeIndex));

    }

    //El hilo del GET
    [System.Obsolete]
    IEnumerator GetPokemon(int numPokemon)
    {

        //Tema peticiones
        #region Peticion
        //Modifica el URL para ponerle pokemon/{numero_en_pokedex}
        string pokemonURL = basePokeURL + "pokemon/" + numPokemon.ToString();

        //Lanza la peticion Get con la url bien formada
        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);

        //Se espera a que la base de datos este cargada
        yield return pokeInfoRequest.SendWebRequest();

        //Comprueba que la base de datos no da ningun fallo
        if(pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        //Obtiene el fragmento del "digimon" que nos interesa
        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        #endregion

        //Da a cada cosa el valor que le corresponde
        #region DarValor
        //Todo lo referente a los textos
        #region DarValoresTextos
        string pokeName = pokeInfo["name"];
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        JSONNode pokeTypes = pokeInfo["types"];
        string[] pokeTypeNames = new string[pokeTypes.Count];

        for(int i = 0, j = pokeTypes.Count - 1; i < pokeTypes.Count; i++, j--)
        {
            pokeTypeNames[j] = pokeTypes[i]["type"]["name"];
        }
        #endregion

        //Todo lo referente al sprite
        #region DarValorSprite

        //Hace una peticion para el sprite
        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);

        //Espera hasta que termine
        yield return pokeSpriteRequest.SendWebRequest();

        //Si falla peta
        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }
        #endregion
        #endregion

        //Asignar a los UIs los datos
        #region AsignarUIs
        SprPokemon.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        SprPokemon.texture.filterMode = FilterMode.Point;

        NombrePokemon.text = CapitalizeFirstLetter(pokeName);

        for (int i = 0; i < TipoPokemon.Length; i++)
        {
            try
            {
                TipoPokemon[i].text = CapitalizeFirstLetter(pokeTypeNames[i]);
            }
            catch (IndexOutOfRangeException)
            {
                TipoPokemon[i].text = "";
            }

        }
        #endregion

    }

    //Un metodo para hacer que la primera letra se ponga en mayuscula
    private string CapitalizeFirstLetter(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}
