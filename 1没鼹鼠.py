import pygame, sys, math, random
from pygame.locals import *
'''
知识点：
1.atan求角度
2.sin cos求二维速度（再次略讲一遍）
3.精灵组碰撞检测
4.时钟
5.血量绘制
6.游戏暂停修复

注意：
'''
local_image = 'resources\\images\\'
local_sound = 'resources\\sounds\\'
#素材
shooter_image = local_image + 'shooter.png'
potato_image = local_image + 'potato.png'
mole_image = local_image + 'mole.png'

healthbar_image = local_image + 'healthbar.png'
health_image = local_image + 'health.png'
background_image = local_image + 'background.png'
win_image = local_image + 'win.png'
game_pause_image = local_image + 'pause.png'
game_over_image = local_image + 'game_over.png'
#音效
game_music_sound = local_sound + 'game_music.wav'
shoot_sound = local_sound + 'shoot.wav'
explode_sound = local_sound + 'explode.wav'
score_sound = local_sound + 'score.wav'
win_sound = local_sound + 'win.wav'
die_sound = local_sound + 'die.wav'

#系统
WINDOW_WIDTH = 800
WINDOW_HEIGHT = 600
FPS = 60

#精灵属性

POTATO_SIZE = (100, 100)    #土豆大小
POTATO_POS = [(20, 0), (20, 100), (20, 200), (20, 300), (20, 400), (20, 500)]  #土豆位置
MOLE_SIZE = (60, 30)        #鼹鼠大小
MOLE_POS = (800-MOLE_SIZE[0], random.randint(0, 600-MOLE_SIZE[1]))  #初始随机生成一个鼹鼠的位置
#鼹鼠速度

MOLE_SPEED = [-5, 0]
GAME_TIME = 30  #游戏时间

WIN_SCORE = 2500  #胜利分数
#提示信息
SCORE_POS = (500, 0)
TIME_POS = (650, 0)
HEALTHBAR_POS = (150, 5)  #血条槽位置
HEALTH_POS = (150+3, 5+3)  #绿色血量条
HEALTH_TEXT_POS = (350, 0)  #血量文字提示
INFO_POS = (300, 250)  #游戏信息提示

# 游戏状态常量定义
GAME_STATE_INIT = 0
GAME_STATE_RUN  = 1
GAME_STATE_PAUSE = 2
GAME_STATE_SUCCESS = 3
GAME_STATE_OVER = 4
GAME_STATE_EXIT = 5
# 游戏暂停标志
GAME_STATE_PAUSE_FLAG = False




#土豆类
class Potato(pygame.sprite.Sprite):
	def __init__(self, position):
		pygame.sprite.Sprite.__init__(self)
		self.image = pygame.image.load(potato_image)
		self.image = pygame.transform.smoothscale(self.image, POTATO_SIZE)
		self.rect = self.image.get_rect()
		self.rect.topleft = position


#鼹鼠类
class Mole(pygame.sprite.Sprite):
	def __init__(self, position, speed=MOLE_SPEED):
		pygame.sprite.Sprite.__init__(self)
		self.image = pygame.image.load(mole_image)
		self.image = pygame.transform.smoothscale(self.image, MOLE_SIZE)
		self.rect = self.image.get_rect()
		self.rect.topleft = position
		self.speed = speed

	def move(self):
		self.rect = self.rect.move(self.speed)


#保卫鼹鼠
class DefendPotato():
	def __init__(self):
		#初始化游戏界面
		pygame.init()
		self.game_font = pygame.font.SysFont('simhei', 24)
		self.screen = pygame.display.set_mode((WINDOW_WIDTH, WINDOW_HEIGHT))
		pygame.display.set_caption('保卫土豆')
		#载入图片
		self.background_image = pygame.image.load(background_image)
		self.win_image = pygame.image.load(win_image)
		self.game_pause_image = pygame.image.load(game_pause_image)
		self.game_over_image = pygame.image.load(game_over_image)
		self.game_over_image.convert_alpha()
		self.healthbar_image = pygame.image.load(healthbar_image)
		self.health_image = pygame.image.load(health_image)
		#设置帧
		self.fps = pygame.time.Clock()

		#初始游戏状态
		self.game_state = GAME_STATE_INIT
		self.game_state_flag = False
		#初始得分
		self.score = 0
		#游戏初始化
#		self.shooter = Shooter(SHOOTER_POS)
		self.potato = pygame.sprite.Group()
		self.mole = pygame.sprite.Group()
		#初始血量
		self.health = 200
		#单位毫秒
		self.time = 0
		self.add_flag = False
		# 游戏音效
		self.background_music = pygame.mixer.music.load(game_music_sound)
		self.score_sound = pygame.mixer.Sound(score_sound)
		self.shoot_sound = pygame.mixer.Sound(shoot_sound)
		self.explode_sound = pygame.mixer.Sound(explode_sound)
		self.win_sound = pygame.mixer.Sound(win_sound)
		self.die_sound = pygame.mixer.Sound(die_sound)

	#初始化
	def init_game(self):
		self.screen.blit(self.background_image, (0, 0))

		#土豆
		for potato in POTATO_POS:
			new_potato = Potato(potato)
			self.screen.blit(new_potato.image, new_potato.rect)
			# 添加进土豆组
			self.potato.add(new_potato)
		self.next_mole()

	#生成下一个鼹鼠
	def next_mole(self):
		# 生成一只鼹鼠
		pos = (800-MOLE_SIZE[0], random.randint(50, 600-MOLE_SIZE[1]))
		new_mole = Mole(pos)
		self.screen.blit(new_mole.image, new_mole.rect)
		# 添加进鼹鼠组
		self.mole.add(new_mole)

	#运行时，射击状态
	def shoot_state(self):
		#画背景和炮塔
		self.screen.blit(self.background_image, (0, 0))

		#画土豆
		for potato in self.potato:
			self.screen.blit(potato.image, potato.rect)
		#画鼹鼠
		for mole in self.mole:
			self.screen.blit(mole.image, mole.rect)
			mole.move()
		#获取箭、鼹鼠状态
#		self.get_bullet_state()
		self.get_mole_state()
		#每过3秒生成一个鼹鼠
		self.set_time()
		# 绘制文字描述信息
		text = "分数：" + str(self.score)
		self.print_text(SCORE_POS, text)
		text = "时间：" + str(GAME_TIME-self.time//1000)
		self.print_text(TIME_POS, text)
		# 添加生命值显示
		self.screen.blit(self.healthbar_image, HEALTHBAR_POS)
		#血条边框三个像素，把绿色血条画到边框里
		for health in range(self.health-6):
			self.screen.blit(self.health_image, (health+HEALTH_POS[0], HEALTH_POS[1]))
		text = "血量：" + str(self.health//2)
		self.print_text(HEALTH_TEXT_POS, text)

	# 时间
	def set_time(self):
		#获取游戏运行的毫秒数
		ticks = pygame.time.get_ticks()
		#处于1000会损失精度
		if ticks > self.time + 1000:
			self.time = ticks
			self.next_mole()



	#判断鼹鼠状态
	def get_mole_state(self):
		for mole in self.mole:
			# 超出屏幕，移除
			if mole.rect.top < 0 or mole.rect.bottom > WINDOW_HEIGHT \
				or mole.rect.left < 0 or mole.rect.right > WINDOW_WIDTH:
				self.mole.remove(mole)
			#撞到土豆，移除，掉血
			if pygame.sprite.groupcollide(self.mole, self.potato, True, False):
				self.health -= 20
				self.explode_sound.play()

	# 游戏状态判断
	def get_game_state(self):
		#时间到分数不够，游戏失败
		if self.time//1000 >= GAME_TIME and self.score < WIN_SCORE:
			self.game_state = GAME_STATE_OVER
			self.die_sound.play()
			pygame.mixer.music.stop()
		#血量为0，游戏失败
		if self.health <= 0:
			self.game_state = GAME_STATE_OVER
			self.die_sound.play()
			pygame.mixer.music.stop()
		#规定时间内拿到规定分数，游戏胜利
		if self.time//1000 <= GAME_TIME and self.score >= WIN_SCORE:
			self.game_state = GAME_STATE_SUCCESS
			self.win_sound.play()
		return self.game_state

	# 打印文字
	def print_text(self, pos, text, color=(0, 0, 0)):
		imgText = self.game_font.render(text, True, color)
		self.screen.blit(imgText, pos)

	# 事件监听
	def listen_game(self):
		# 事件监听
		for event in pygame.event.get():
			if event.type == QUIT:
				self.game_state = GAME_STATE_EXIT
			# 空格键-暂停
			if event.type == KEYUP:
				if event.key == K_SPACE:
					self.game_state_flag = not self.game_state_flag
					if self.game_state == GAME_STATE_RUN and self.game_state_flag:
						self.game_state = GAME_STATE_PAUSE
					elif self.game_state == GAME_STATE_PAUSE:
						self.game_state = GAME_STATE_RUN


	#游戏流程控制
	def control_game(self):
		# 初始时
		if self.game_state == GAME_STATE_INIT:
			self.init_game()
			self.game_state = GAME_STATE_RUN
		# 运行时
		elif self.game_state == GAME_STATE_RUN:
			self.shoot_state()
			self.get_game_state()
		# 暂停时
		elif self.game_state == GAME_STATE_PAUSE:
			# 绘制背景
			self.screen.blit(self.game_pause_image, (0, 0))
		# 结束时
		elif self.game_state == GAME_STATE_OVER:
			# 绘制背景
			self.screen.blit(self.game_over_image, (0, 0))
		# 胜利时
		elif self.game_state == GAME_STATE_SUCCESS:
			# 绘制背景
			self.screen.blit(self.win_image, (0, 0))
		elif self.game_state == GAME_STATE_EXIT:
			sys.exit()

	#主程序
	def run(self):
		pygame.mixer.music.play(-1)
		# 游戏主循环
		while True:
			print(self.game_state)
			#事件监听
			self.listen_game()
			# 游戏控制流程
			self.control_game()
			#设置帧
			self.fps.tick(FPS)
			pygame.display.update()

#运行游戏
app = DefendPotato()
app.run()
