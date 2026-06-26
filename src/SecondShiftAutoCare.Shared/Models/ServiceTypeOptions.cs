namespace SecondShiftAutoCare.Shared.Models;

public static class ServiceTypeOptions
{
    public const string NotSure = "Not Sure / Diagnose It";

    public static readonly string[] Core =
    [
        "Oil Change",
        "Brakes / Rotors",
        "Starter",
        "Serpentine Belt",
        "Spark Plugs",
        "Suspension Diagnosis",
        "Ball Joint",
        "Wheel Bearing",
        "CV Axle",
        "General Diagnosis"
    ];

    public static readonly string[] Additional =
    [
        "Battery Replacement",
        "Battery / Charging System Test",
        "Alternator Replacement",
        "Brake Caliper Replacement",
        "Brake Hose Replacement",
        "Tire Rotation",
        "Headlight / Taillight Bulb Replacement",
        "Cabin Air Filter",
        "Engine Air Filter",
        "Wiper Blades",
        "Thermostat Replacement",
        "Water Pump Replacement",
        "Radiator Hose Replacement",
        "Coolant Flush",
        "Transmission Drain / Fill",
        "Differential Fluid Service",
        "Transfer Case Fluid Service",
        "Sway Bar Links",
        "Tie Rod Ends",
        "Control Arms",
        "Struts / Shocks",
        "Exhaust Leak Inspection",
        "Check Engine Light Basic Diagnosis",
        "No-Start Diagnosis",
        "AC Basic Diagnosis",
        "Pre-Trip Inspection",
        "Used Vehicle Inspection"
    ];

    public static readonly string[] All = [NotSure, .. Core, .. Additional];
}
