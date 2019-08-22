docker container rm indy_pool
docker build -f ../docker/indy-pool.dockerfile -t indy_pool .
docker run --name indy_pool -itd -p 9701-9709:9701-9709 indy_pool 