# Instrumentation Spike
This spike will help to Monitor you application activity by producing performence counter Telematry.

Easily add Monitors to your application and raise monitor events. 
This will create perfmon counter/s in the background that can be used for monitoring or alterting.

Basic Types
- Basic Monitor:          Counter that can be incremented or decrimented
- Average Monitor:        Counter that checks the average over time (10min - 1 hour)
- Heartbeat:              Is alive counter
- PulseMonitor:           Checks for state changes i.e. when was the last time

Complex Monitors
- Two State:              This minitors used various basic counters to track behavior of an event that can have two states i.e. sucess or failure.

