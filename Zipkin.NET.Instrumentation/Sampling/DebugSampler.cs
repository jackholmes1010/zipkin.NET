namespace Zipkin.NET.Instrumentation.Sampling
{
	public class DebugSampler : ISampler
	{
		public bool IsSampled(string traceId)
		{
			return true;
		}
	}
}
