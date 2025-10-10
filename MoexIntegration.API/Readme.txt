
1) открыть консоль в решении проекта

2) собрать новый образ:
docker build -t fandrey/moexintegration .

3) отправить новый образ в Docker Hub:
docker push fandrey/moexintegration:latest

4) на виртуальном сервере: обновить образ
docker pull fandrey/moexintegration:latest

5) на виртуальном сервере: остановить существующий контейнер
docker stop moexintegration

6) на виртуальном сервере: удалить старый контейнер из системы
docker rm moexintegration

7) на виртуальном сервере: запустить новый образ
docker run -d -p 8080:8080 -p 8081:8081 --name moexintegration fandrey/moexintegration:latest