# Observability

| **Name** | **Date**   | **Commentary** |
|----------|------------|-----------------|
| VS       | 15/11/2025 | Initialisation  |
|          |            |                 |
|          |            |                 |

## Introduction
Improve with mimir https://github.com/grafana/mimir/blob/main/docs/sources/mimir/get-started/play-with-grafana-mimir/docker-compose.yml
## Jaeger

```
podman run --rm --name jaeger -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 -p 16686:16686 -p 4317:4317 -p 4318:4318 -p 14250:14250 -p 14268:14268 -p 14269:14269 -p 9411:9411 jaegertracing/all-in-one:1.74.0
```

## Prometheus

```
podman run --add-host=host.containers.internal:host-gateway -d --name prometheusNet -p 9090:9090 -v ./Configs/prometheus.yml:/etc/prometheus/prometheus.yml prom/prometheus --config.file=/etc/prometheus/prometheus.yml
```


## Grafana tools

### Grafana (le cœur / UI)

Le serveur Grafana lui-même : UI, dashboards, alerting, data sources, etc.
Permet de connecter des sources de données (“data sources”) comme Loki, Tempo, Mimir, Pyroscope, etc.

### Grafana Mimir

Base de données de métriques (TSDB) hautement scalable, compatible Prometheus. Grafana Labs
Permet du stockage long terme, multi-tenant, haute disponibilité. Grafana Labs
Très utile quand tu veux stocker beaucoup de métriques Prometheus ou depuis un Agent.

### Grafana Loki

Système de logs. Loki permet de stocker des logs avec des labels (plutôt que d’indexer tout le contenu), ce qui le rend plus “léger” / économique. Grafana Labs+1
On l’utilise souvent avec Promtail (ou d’autres agents) pour ingérer des logs, et ensuite on les visualise dans Grafana.

### Grafana Tempo

Backend de traces distribuées (distributed tracing). Grafana Labs
Permet d’ingérer via des protocoles comme OpenTelemetry, Jaeger, Zipkin. Grafana Labs+1
L’interface Tempo peut être connectée comme une “data source” dans Grafana pour faire des requêtes de traces. Grafana Labs

### Grafana Pyroscope

Outil de “continuous profiling” (profilage continu) : collecte des profils (CPU, mémoire) de tes applications au fil du temps. Grafana Labs+1
Multi-tenant, scalable et intégré à Grafana pour corréler les profils avec métriques, logs et traces. Grafana Labs
Très utile pour comprendre sur quelles parties de ton code ton application passe le plus de temps ou consomme plus de ressource.

### Grafana Beyla

Instrumentation automatique basée sur eBPF. Grafana Labs
Permet de capturer des métriques RED (Rate, Error, Duration) + des traces sans modifier le code de ton application (ce qui est très pratique pour “observer sans instrumenter manuellement”). Grafana Labs
Compatible avec plusieurs langages : Go, Java, Python, Rust, .NET, etc. Grafana Labs
Les données collectées (métriques + traces) peuvent être envoyées à un Agent / Alloy + Mimir / Tempo.

### Grafana Alloy

Remplace progressivement le Grafana Agent. Dans la revue open source 2024, Grafana annonce Alloy comme “distrib OTel collector” moderne. Grafana Labs
C’est une distribution de l’OpenTelemetry Collector, avec des pipelines pour métriques, logs, traces, et éventuellement profils. Grafana Labs
On recommande de migrer du Grafana Agent vers Alloy : l’Agent entre dans sa phase de fin de vie (“end-of-life”) le 1er novembre 2025. Grafana Labs+1
Alloy permet de collecter des données (“télémetry”) depuis les applications / infrastructure et de les envoyer vers les backends Grafana (Mimir, Loki, Tempo, Pyroscope).
