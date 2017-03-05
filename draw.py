import matplotlib.pyplot as plt
class drawer:
    def readData(filename):
        f = open(filename,"r")
        data = []
        for line in f.readlines()
            data.append(line)
        return data

    def getRewards(data):
        rewards = []
        for epod,reward in data:
            rewards.append(reward)
        return rewards
            
            
        
    def draw(data):
        plt.plot(data)
        plt.ylabel('rewards')
        plt.show()
    

