# IoT Services

### Web Server
* Port 80
* REST API server
* Razor pages

### SMTP Server
* Port 587
* Process Camera email for motion detection.

### MQTT Broker
* Port 1883
* Publish and subscribe with topic and messages.
* Set `IOT_MQTT_KEEP_ALIVE` to true for socket keep alive feature.

## Features
* Publish MQTT messages to connected clients when Swann NVR detects motion.
  * Email subject is configurable by specifying environment variable `SWANN_EMAIL_SUBJECTS`, multiple subjects separated by semicolon.

* API to log temperature sensor data to InfluxDB2. Configure environment variables for connecting to InfluxDB2:
  * `IOT_LOGGER_INFLUX_DB_URL`
  * `IOT_LOGGER_INFLUX_DB_TOKEN`
  * `IOT_LOGGER_INFLUX_DB_TOKEN`
  * `IOT_LOGGER_INFLUX_TEMP_ORG`
  * `IOT_LOGGER_INFLUX_TEMP_BUCKET`
