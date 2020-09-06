using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightIntensityChanger : MonoBehaviour 
{
	
	[SerializeField] private float _minIntensity;
	[SerializeField] private float _maxIntensity;

	[SerializeField] private float _minRange;
	[SerializeField] private float _maxRange;

	[SerializeField] private float _changeSpeed;

	private Light _light;

	private void Awake()
	{
		_light = GetComponent<Light>();

		StartCoroutine(IntensityChanger());
	}

	private IEnumerator IntensityChanger()
	{
		float intensityDifference = _maxIntensity - _minIntensity;
		float rangeDifference = _maxRange - _minRange;

		while (true)
		{
			float perlinNoise = Mathf.PerlinNoise(Time.time * _changeSpeed, 0);
			
			_light.intensity = _minIntensity + perlinNoise * intensityDifference;
			_light.range = _minRange + perlinNoise * rangeDifference;

			yield return null;
		}
	}
}
