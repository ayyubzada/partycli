namespace Party.Application.DTOs;

public record ServerDto(string Name, int Load, string Status, ServerLocationDto[] Locations);

public record ServerLocationDto(int Id, ServerLocationCountryDto Country);

public record ServerLocationCountryDto(int Id, string Name, string Code);