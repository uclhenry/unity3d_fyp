import pygame, sys, math, random
from pygame.locals import *

local_image = 'resources\\images\\'
local_sound = 'resources\\sounds\\'
#素材
potato_image = local_image + 'potato.png'

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
GAME_TIME = 30  #游戏时间

WIN_SCORE = 2500  #胜利分数
#提示信息
SCORE_POS = (500, 0)
TIME_POS = (650, 0)

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
		#单位毫秒
		self.time = 0
		self.add_flag = False
		# 游戏音效
		self.background_music = pygame.mixer.music.load(game_music_sound)
		self.score_sound = pygame.mixer.Sound(score_sound)
		self.shoot_sound = pygame.mixer.Sound(shoot_sound)
		self.explode_sound = pygame.mixer.Sound(explode_sound)
		self.win_sound     = pygame.mixer.Sound(win_sound)
		self.die_sound     = pygame.mixer.Sound(die_sound)
	#初始化
	def init_game(self):
		self.screen.blit(self.background_image, (0, 0))
		#土豆
		for potato in POTATO_POS:
			new_potato = Potato(potato)
			self.screen.blit(new_potato.image, new_potato.rect)
			# 添加进土豆组
			self.potato.add(new_potato)
	#运行时，射击状态
	def shoot_state(self):
		#画背景和炮塔
		self.screen.blit(self.background_image, (0, 0))
		#画土豆
		for potato in self.potato:
			self.screen.blit(potato.image, potato.rect)
		#每过3秒生成一个鼹鼠
		self.set_time()
		# 绘制文字描述信息
		text = "分数：" + str(self.score)
		self.print_text(SCORE_POS, text)
		text = "时间：" + str(GAME_TIME-self.time//1000)
		self.print_text(TIME_POS, text)

	# 时间
	def set_time(self):
		#获取游戏运行的毫秒数
		ticks = pygame.time.get_ticks()
		#处于1000会损失精度
		if ticks > self.time + 1000:
			self.time = ticks
	# 游戏状态判断
	def get_game_state(self):
		#时间到分数不够，游戏失败
		if self.time//1000 >= GAME_TIME and self.score < WIN_SCORE:
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