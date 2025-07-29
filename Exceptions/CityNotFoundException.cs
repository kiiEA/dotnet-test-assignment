namespace WeatherMcpServer.Exceptions;

public class CityNotFoundException(string message) : Exception(message);
public class InvalidTokenException(string message) : Exception(message);
public class RateLimitExceededException(string message) : Exception(message);
public class ApiException(string message) : Exception(message);